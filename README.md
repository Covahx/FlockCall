# FlockCall

Minimal offline web app: a small set of merganser call buttons for careful short-term use with a stressed duckling. See `HOW_TO_RUN.md` for ethics, limits, and Android paths.

## Open in any phone browser (GitHub Pages)

After you enable Pages and push to `main`, the app is served here:

**[https://covahx.github.io/FlockCall/](https://covahx.github.io/FlockCall/)**

1. In the repo on GitHub: **Settings** → **Pages** → under **Build and deployment**, set **Source** to **GitHub Actions** (not “Deploy from a branch”).
2. Push to **`main`** (or run the **Deploy GitHub Pages** workflow manually under **Actions**).
3. When the workflow finishes, open the link above on your phone. Use **Add to Home screen** if you want a full-screen shortcut.

Source repo: [github.com/Covahx/FlockCall](https://github.com/Covahx/FlockCall)

## Project layout

| Path | Purpose |
|------|--------|
| `app/` | Static site (`index.html`, `sw.js`, `assets/`) — this folder is what Pages publishes |
| `HOW_TO_RUN.md` | Run locally, replace audio, GitHub Pages, APK notes |
| `tools/FlockCallTestServer/` | Optional local test server (build with `build_FlockCallTestServer.bat`) |

Audio clips belong under `app/assets/audio/` (see `app/assets/audio/README.txt`).
