using System.Diagnostics;
using System.Net;
using System.Text;

namespace FlockCallTestServer;

internal static class Program
{
    private static string? ResolveAppRoot()
    {
        var cli = Environment.GetCommandLineArgs();
        if (cli.Length >= 2)
        {
            var p = Path.GetFullPath(cli[1].Trim('"'));
            if (File.Exists(Path.Combine(p, "index.html")) && File.Exists(Path.Combine(p, "sw.js")))
                return p;
        }

        var seeds = new List<string>();
        var ep = Environment.ProcessPath;
        if (!string.IsNullOrEmpty(ep))
        {
            var d = Path.GetDirectoryName(ep);
            if (d != null) seeds.Add(d);
        }
        seeds.Add(AppContext.BaseDirectory);
        foreach (var start in seeds.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var x = start;
            for (var i = 0; i < 14 && !string.IsNullOrEmpty(x); i++)
            {
                var nested = Path.Combine(x, "app", "index.html");
                if (File.Exists(nested))
                    return Path.GetFullPath(Path.Combine(x, "app"));
                if (File.Exists(Path.Combine(x, "index.html")) &&
                    File.Exists(Path.Combine(x, "sw.js")))
                    return Path.GetFullPath(x);
                x = Directory.GetParent(x)?.FullName ?? "";
            }
        }
        return null;
    }

    private static string Mime(string ext) => ext.ToLowerInvariant() switch
    {
        ".html" => "text/html; charset=utf-8",
        ".htm" => "text/html; charset=utf-8",
        ".js" => "text/javascript; charset=utf-8",
        ".json" => "application/json; charset=utf-8",
        ".svg" => "image/svg+xml",
        ".txt" => "text/plain; charset=utf-8",
        ".css" => "text/css; charset=utf-8",
        ".xml" => "application/xml",
        ".webmanifest" => "application/manifest+json",
        ".mp3" => "audio/mpeg",
        ".mp4" => "video/mp4",
        ".m4a" => "audio/mp4",
        ".wav" => "audio/wav",
        ".ogg" => "audio/ogg",
        ".ico" => "image/x-icon",
        ".woff" => "font/woff",
        ".woff2" => "font/woff2",
        _ => "application/octet-stream",
    };

    private static void Send(HttpListenerResponse res, int code, string body)
    {
        res.StatusCode = code;
        res.ContentType = "text/plain; charset=utf-8";
        var b = Encoding.UTF8.GetBytes(body);
        res.OutputStream.Write(b);
    }

    public static void Main()
    {
        var root = ResolveAppRoot();
        if (root == null)
        {
            Console.Error.WriteLine("Could not find FlockCall web root.");
            Console.Error.WriteLine("Put FlockCallTestServer.exe in the project folder next to the \"app\" directory,");
            Console.Error.WriteLine("or inside a parent folder that contains app\\index.html.");
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            Environment.Exit(1);
            return;
        }

        using var listener = new HttpListener();
        var port = 0;
        for (var p = 8765; p < 8810; p++)
        {
            listener.Prefixes.Clear();
            listener.Prefixes.Add($"http://127.0.0.1:{p}/");
            try
            {
                listener.Start();
                port = p;
                break;
            }
            catch (HttpListenerException) { /* try next port */ }
        }

        if (port == 0)
        {
            Console.Error.WriteLine("No free port found between 8765 and 8809.");
            Console.ReadLine();
            Environment.Exit(1);
            return;
        }

        var url = $"http://127.0.0.1:{port}/";
        Console.WriteLine("FlockCall test server");
        Console.WriteLine($"  Folder: {root}");
        Console.WriteLine($"  Open:   {url}");
        Console.WriteLine("  Close this window or press Ctrl+C to stop.");
        Console.WriteLine();

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("(Could not start browser automatically: " + ex.Message + ")");
            Console.WriteLine("Open the URL above manually.");
        }

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            try { listener.Stop(); } catch { /* ignore */ }
        };

        try
        {
            while (listener.IsListening)
            {
                var ctx = listener.GetContext();
                try
                {
                    var req = ctx.Request;
                    var res = ctx.Response;
                    var raw = req.Url?.AbsolutePath ?? "/";
                    if (raw == "/" || string.IsNullOrEmpty(raw))
                        raw = "/index.html";
                    var rel = raw.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                    rel = Uri.UnescapeDataString(rel);
                    var full = Path.GetFullPath(Path.Combine(root, rel));
                    var rootFull = Path.GetFullPath(root);
                    if (!full.StartsWith(rootFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(full, rootFull, StringComparison.OrdinalIgnoreCase))
                    {
                        Send(res, 403, "Forbidden");
                        continue;
                    }

                    if (Directory.Exists(full) && !File.Exists(full))
                    {
                        var idx = Path.Combine(full, "index.html");
                        if (File.Exists(idx)) full = idx;
                    }

                    if (!File.Exists(full))
                    {
                        Send(res, 404, "Not found");
                        continue;
                    }

                    res.ContentType = Mime(Path.GetExtension(full));
                    res.StatusCode = 200;
                    using var fs = File.OpenRead(full);
                    fs.CopyTo(res.OutputStream);
                }
                catch (Exception)
                {
                    try
                    {
                        Send(ctx.Response, 500, "Error");
                    }
                    catch { /* ignore */ }
                }
                finally
                {
                    try { ctx.Response.OutputStream.Close(); } catch { /* ignore */ }
                }
            }
        }
        catch (HttpListenerException)
        {
            /* listener.Stop() during GetContext */
        }
        finally
        {
            try { listener.Close(); } catch { /* ignore */ }
        }
    }
}
