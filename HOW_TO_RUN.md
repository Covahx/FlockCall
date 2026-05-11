# Running FlockCall on Android

You do not need a developer toolchain. Pick one path. The first option is the easiest and works for most cases. The third option produces a real installable APK.

## What's in the `app/` folder

```
app/
  index.html         the entire app, one file
  manifest.json      makes it installable as a home-screen app
  sw.js              service worker, lets it work offline after first load
  icon.svg           app icon
  assets/audio/      drop real merganser .m4a files here when sourced
```

If the audio folder is empty, the app plays soft synthesized placeholder tones with a yellow banner reminding you to replace them. Do not use the placeholders with a real duckling.

---

## Path A: Host it free on GitHub Pages (recommended)

This repo is set up to publish the **`app/`** folder with **GitHub Actions**. Your public URL is:

**[https://covahx.github.io/FlockCall/](https://covahx.github.io/FlockCall/)**

Steps (first time only):

1. Push this repository to GitHub ([Covahx/FlockCall](https://github.com/Covahx/FlockCall)) with the default branch named **`main`**.
2. On GitHub: **Settings** → **Pages** → **Build and deployment** → set **Source** to **GitHub Actions** (not “Deploy from a branch”).
3. Open **Actions**, wait for **Deploy GitHub Pages** to finish (or trigger it with **Run workflow**).
4. On any phone, open **https://covahx.github.io/FlockCall/** in the browser. Use **Add to Home Screen** / **Install app** for a full-screen shortcut. After the first load, the service worker can cache the app for offline use.

To transfer to another phone: send the same URL and repeat **Add to Home Screen** if you want an icon. Works because the service worker caches everything on first load.

---

## Path B: Copy the folder onto the phone over USB

Use this when you cannot or do not want to host anything.

1. Connect the Android phone via USB. Allow file access.
2. Copy the entire `app/` folder to the phone, for example to `Internal storage/FlockCall/`.
3. Install a free local web server app from the Play Store. "Simple HTTP Server" or "Servediter" work. Many file manager apps also include a "browse as web server" option.
4. Point the server at `Internal storage/FlockCall/`.
5. Open the address the server shows (usually `http://localhost:8080`) in Chrome on the same phone.
6. "Add to Home Screen" as above.

You can open `file:///sdcard/FlockCall/index.html` in Chrome to check the **UI**, but **recordings will not load** from `file://`; use the local http server address from step 5 for real audio. `file://` also disables the service worker (no offline caching, and "Install app" will not appear).

Transferring: copy the folder over USB to the new phone, repeat.

---

## Path C: Wrap it as an APK with PWABuilder

Use this when you want a real `.apk` file you can install like any other Android app and email or AirDrop to other phones.

1. Complete Path A so the app is live (for this repo: `https://covahx.github.io/FlockCall/`).
2. Open https://www.pwabuilder.com/ in a desktop browser.
3. Paste the GitHub Pages URL. Run the analysis.
4. Choose "Package For Stores," then Android. Use the default options. Generate.
5. Download the zip. Inside is a file called something like `app-release-signed.apk` (or `.aab`).
6. Email or transfer the `.apk` to the Android phone.
7. On the phone, tap the `.apk`. Android will ask you to allow installs from this source the first time. Allow it. The app installs.

Transferring to another phone: send the same `.apk` file by any means (email, Drive, Bluetooth, USB). Install on the new phone the same way. No URL or hosting needed.

---

## Replacing the placeholder audio

Source real, ethically-licensed merganser calls (see `app/assets/audio/README.txt` for details). Encode them as mono AAC `.m4a` at 96 to 128 kbps, 0.8 to 2.5 seconds each, peak normalized to -6 dBFS.

Drop them into `app/assets/audio/` using the expected filenames listed in that README. Refresh the app (or reinstall the APK). The yellow placeholder banner disappears once real audio is loaded.

---

## Updating the app after first install

Path A and C: rebuild and replace the hosted/APK version. The service worker will fetch the update on next launch when online.

Path B: replace the folder on the phone, reload in Chrome.

---

## Quick sanity check on a desktop browser

Opening `app/index.html` as a **disk file** (`file:///…`) only checks the **UI and rules**. **Your own audio files will not load** in Chrome or Edge from `file://` (the browser blocks those requests), so you will only hear the built-in test tones.

To hear real clips on the desktop, serve the `app` folder over **http**:

### Option 1: double-click launcher (Windows)

1. One-time: run **`build_FlockCallTestServer.bat`** in the project root (needs the [.NET 9 SDK](https://dotnet.microsoft.com/download)). This writes **`dist/FlockCallTestServer.exe`** (~35 MB, self-contained; no separate Python runtime).
2. Whenever you want to test: double-click **`StartFlockCallTest.bat`**. A console window starts the server and your default browser should open to **http://127.0.0.1:8765/** (or the next free port). Close the console window to stop.

Keep the `.exe` inside the FlockCall project tree so it can find the **`app`** folder. If you move only the `.exe` elsewhere, pass the path to `app` as the first argument, for example: `FlockCallTestServer.exe "D:\FlockCall\app"`.

### Option 2: Python

1. Double-click `app/serve_local.bat` (needs [Python](https://www.python.org/) on PATH), **or** in a terminal: `cd` into the `app` folder and run `py -m http.server 8765`.
2. In Edge, open **http://localhost:8765/** (not a `file:///…` address).

The service worker registers on `http://localhost` / `http://127.0.0.1` as usual.
