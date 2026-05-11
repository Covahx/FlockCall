# FlockCall

A minimal, ethics-first companion app for short-term care of a rescued juvenile common merganser (storskrake). The app plays five soft, behaviorally appropriate calls intended to reduce stress and support natural orientation during temporary rehabilitation. It is deliberately tiny, deliberately quiet, and deliberately designed to be used less and less over time.

This document is the full concept, UI plan, technical architecture, and solo-developer roadmap.

## 1. Guiding Principles

These come before any feature decision and override any UX convenience.

1. The duckling is the user, not the human. Every feature is judged against whether it supports a wild-bird outcome.
2. Less is better. The app should be quiet, brief, and easy to put down.
3. No imprinting. Sounds are species-typical, not human-flavored. Cooldowns are enforced.
4. Rehabilitators first. The first screen on first launch points to professional wildlife rehab contacts.
5. Offline-first. No network is required after install. No analytics, no ads, no cloud calls.
6. Solo-developer scope. If a feature cannot be shipped by one person in a weekend, it is out.

## 2. App Concept

A single screen with five large pads. Each pad represents one call category. Tapping a pad plays a short, natural-sounding clip once. A small global status bar shows the current cooldown state and a soft suggested daily session counter. A small "Info" affordance opens a single static page with ethics guidance and rehabilitator links.

The app does not loop. The app does not autoplay. The app does not gamify. The app has no notifications. The app has no streaks.

### Call Categories

1. Contact / reassurance call. Short, soft, low-energy. Used when the duckling is alone or panicking.
2. Gather / follow call. Slightly more rhythmic. Used to help orient the duckling near the carer during transfer.
3. Feeding / calm social chatter. Low murmuring, conversational, gentle. Played near feeding time only.
4. Warning / danger alert. Sharper. Used very rarely. Hidden behind a confirmation tap to prevent casual use.
5. Location / flock response. Medium-distance call, soft. Used outdoors near water to encourage natural orientation.

## 3. UI Layout

One primary screen. Five buttons in a 2-3 or 3-2 grid, large enough for outdoor one-handed use with wet fingers. Dark calm background. No decorative motion. No splash screen beyond a 600 ms fade.

```
+--------------------------------------+
|  FlockCall                       (i) |
|  cooldown ready                      |
+--------------------------------------+
|                                      |
|   [  Contact  ]   [  Gather   ]      |
|                                      |
|   [ Feeding ]   [  Warning  ]        |
|                                      |
|         [  Location  ]               |
|                                      |
+--------------------------------------+
|  session: 3 / suggested daily 8      |
+--------------------------------------+
```

Buttons are not labeled with cute icons. Plain text plus a small monochrome glyph. Warning has a thin amber border to discourage idle taps. All other buttons share the same calm slate tone.

### Wireframe Descriptions

Main screen. Status row top left ("ready" or "cooldown 00:42"). Info dot top right. Five rounded rectangles, generous padding, 64 dp minimum touch height, 24 dp text. Bottom row shows today's play count with a soft suggested cap.

Info screen. Single scrollable page. Plain prose. No images. Top of page: "If you have found a wild duckling, contact a licensed wildlife rehabilitator first." Below: list of contact resources by region (user editable in settings, or shipped as a static list for the developer's region only in MVP). Below that: short ethics statement, anti-imprinting note, volume guidance, and a reminder that this app is a stopgap, not a substitute for rehab.

First-launch screen. Same as Info screen, but with a single button at the bottom: "I understand and will reduce app use as the duckling stabilizes." Recorded as a one-time acknowledgement.

## 4. Color Palette

Dark, calm, low-saturation. Designed to be readable outdoors without being bright.

- Background: #0E1418 (deep slate, slight blue undertone)
- Surface (button face): #1B2329
- Surface elevated (active): #243038
- Text primary: #D6DCE0
- Text muted: #7C868E
- Accent calm: #4A6B7C (soft teal-gray)
- Accent caution: #B58A4A (warm amber, used only on Warning button border)
- Accent ready: #5F8A6B (soft green, used only for the small "ready" status dot)

No gradients. No shadows beyond a 1 px inner stroke. No animation beyond a 120 ms button press dim.

## 5. Recommended Sound Behavior

Each call is a short clip, 0.8 to 2.5 seconds. No loops. No fades that suggest continuity.

- Sample rate: 44.1 kHz, mono.
- Format: AAC in .m4a, or Ogg Vorbis, encoded around 96 to 128 kbps. Avoid MP3 for licensing simplicity; AAC is fine on Android and iOS.
- Peak normalize to -6 dBFS. Do not loudness-maximize. Quiet is the point.
- Each category has 2 to 4 variants. The playback manager rotates variants randomly without immediate repeats to avoid mechanical repetition that would feel unnatural to the bird.
- The Warning clip is a single variant only, kept deliberately rare.

### Volume Limiter Recommendation

The app cannot reliably cap absolute SPL across all devices, but it can soft-limit:

- On launch, system media volume is read. If above 60 percent, a one-time toast suggests lowering it.
- The app applies an internal output gain of 0.6 by default. A settings toggle allows 0.4 (quieter) or 0.7 (outdoors). No higher option is exposed.
- Recommended use case in the Info screen: keep device speaker at arm's length from the duckling, never directed at the bird from close range.

## 6. Playback Timing Rules

These are hard rules enforced in code, not suggestions.

- Per-button cooldown: 45 seconds between plays of the same category.
- Global cooldown: 8 seconds between any two plays, regardless of category.
- Warning cooldown: 5 minutes, plus a confirmation tap required.
- Daily soft cap: 8 sessions suggested. After 8, the status bar gently dims and shows "consider resting." Playback is not blocked, only discouraged.
- Daily hard cap: 20 plays. After 20, all buttons are disabled until local midnight, with a one-line message: "rest period; the duckling benefits from quiet."
- Quiet hours: between 22:00 and 06:00 local time, all buttons require a long press of 1 second to play, to prevent accidental nighttime taps.

A "taper mode" toggle in settings increases cooldowns by 50 percent each enabled day for a week, then prompts the user to uninstall. This operationalizes the principle that the app should encourage its own retirement.

## 7. Anti-Imprinting Safeguards

1. No human voice anywhere in the audio set.
2. Sounds are species-typical merganser or close-relative calls only, not generic "cute duck" sounds.
3. No reward loop. No animations on button press. No haptics beyond a single short tap.
4. Cooldowns and daily caps as above.
5. The carer is encouraged in the Info panel to keep visual and vocal human presence minimal during playback.
6. Taper mode actively reduces availability over time.
7. The app records nothing about the duckling, takes no photos, and offers no "bonding" or "training" framing in copy.

## 8. Technical Architecture

Recommended stack: React Native with Expo, or a thin web app wrapped with Capacitor. Either is achievable solo. Flutter is also fine if the developer already knows it. The decision below assumes React Native with Expo because audio handling and offline packaging are straightforward there.

Core modules:

- UI layer: a single screen plus an Info screen. React Native components, no navigation library needed beyond a simple conditional render or React Navigation's stack with two routes.
- Audio layer: `expo-av` for playback. All clips bundled in the app binary so no runtime download is needed.
- State layer: a small in-memory store (Zustand or plain React context) for cooldown timers and counters. Persistence via `AsyncStorage` for daily counts, last-played timestamps, and taper mode state.
- Time layer: `Date.now()` for cooldowns. Local midnight reset computed from device time. No server time.
- Settings layer: a tiny settings object persisted as one JSON blob.

No backend. No analytics. No crash reporter unless the developer needs one for personal debugging, in which case use something local-only.

## 9. MVP Feature List

Ship these and stop.

1. Five buttons, five sound categories, 2 to 4 variants per category bundled in the app.
2. Per-button and global cooldowns as specified.
3. Daily soft cap, hard cap, and quiet-hours long-press.
4. Warning confirmation tap.
5. Info screen with rehabilitator-first message and ethics statement.
6. First-launch acknowledgement.
7. Volume limiter default gain plus three-option setting.
8. Taper mode toggle.
9. Fully offline after install.
10. Android build that installs as an APK.

Out of scope for MVP: iOS build, theming, multiple languages, multiple bird species, recording, cloud sync, widgets.

## 10. Folder Structure

```
flockcall/
  app.json
  package.json
  README.md
  /assets
    /audio
      contact_01.m4a
      contact_02.m4a
      gather_01.m4a
      gather_02.m4a
      feeding_01.m4a
      feeding_02.m4a
      warning_01.m4a
      location_01.m4a
      location_02.m4a
    /icons
      glyph_contact.svg
      glyph_gather.svg
      glyph_feeding.svg
      glyph_warning.svg
      glyph_location.svg
  /src
    /components
      CallButton.tsx
      StatusBar.tsx
      InfoPanel.tsx
      FirstLaunchGate.tsx
    /audio
      AudioManager.ts
      clipRegistry.ts
    /state
      useSessionStore.ts
      persistence.ts
    /rules
      cooldowns.ts
      caps.ts
      taper.ts
    /screens
      MainScreen.tsx
      InfoScreen.tsx
    /theme
      colors.ts
      spacing.ts
    /utils
      time.ts
    App.tsx
  /docs
    ethics.md
    sourcing.md
```

## 11. Audio Format Handling

Bundle audio as `.m4a` (AAC) at 96 to 128 kbps mono, 44.1 kHz. Total bundle for nine to twelve clips at 1 to 2 seconds each should land well under 2 MB. This keeps install size small and removes any need for on-demand download.

Loading strategy: preload all clips into memory on app start using `expo-av`'s `Audio.Sound.createAsync` with `{ shouldPlay: false }`. Keep references in a registry keyed by clip id. Playback path is then a single `replayAsync` call which is fast enough to feel instant.

Avoid streaming. Avoid background audio sessions. Avoid mixing with other media; on Android, request ducking off and use the media stream so the system volume slider behaves as expected.

## 12. Offline Caching Strategy

There is no cache in the traditional sense because there are no network assets. All audio ships in the APK. The only persistent state is:

- `lastPlayedAt` per category (timestamp).
- `lastGlobalPlayAt` (timestamp).
- `dailyPlayCount` and `dailyDate` (string).
- `taperMode` (object: enabled, startDate).
- `firstLaunchAcknowledged` (boolean).
- `outputGain` (number, one of 0.4, 0.6, 0.7).

All stored as a single JSON blob in `AsyncStorage` under key `flockcall.state.v1`. Read once at startup. Write debounced by 250 ms on changes.

## 13. Prototype Code Architecture

Three layers, kept thin.

- Rules layer is pure functions. Given current state and a requested action, it returns either "allow" or "deny with reason." This is unit testable without any audio or UI.
- Audio layer is a thin wrapper around `expo-av` that knows how to preload, pick a non-repeating variant, and play once. It does not know about cooldowns.
- UI layer wires the two together. The button asks the rules layer, then calls the audio layer, then updates state.

This separation means cooldown logic can be tested with plain Jest and the audio layer can be swapped (for a web prototype) without changing the rules.

## 14. Naming Conventions

Simple, scalable, no cleverness.

- Files: `PascalCase.tsx` for components and screens, `camelCase.ts` for utilities and stores.
- Audio clips: `<category>_<NN>.m4a`, where category is one of `contact`, `gather`, `feeding`, `warning`, `location` and NN is a two-digit variant index.
- Store keys: `flockcall.state.v1`. Bump the version suffix if the schema ever changes.
- Constants in `SCREAMING_SNAKE_CASE` inside `rules/`.
- No abbreviations in public names. Inside a single function, short locals are fine.

## 15. Pseudocode: Sound Playback Manager

```
CATEGORIES = ["contact", "gather", "feeding", "warning", "location"]

PER_BUTTON_COOLDOWN_MS = 45_000
GLOBAL_COOLDOWN_MS     = 8_000
WARNING_COOLDOWN_MS    = 300_000
DAILY_SOFT_CAP         = 8
DAILY_HARD_CAP         = 20
QUIET_START_HOUR       = 22
QUIET_END_HOUR         = 6

function canPlay(category, state, now):
  if state.firstLaunchAcknowledged is false:
    return deny("first launch not acknowledged")

  if dailyDateChanged(state.dailyDate, now):
    state = resetDaily(state, now)

  if state.dailyPlayCount >= DAILY_HARD_CAP:
    return deny("daily rest period")

  if now - state.lastGlobalPlayAt < GLOBAL_COOLDOWN_MS:
    return deny("global cooldown")

  lastForCat = state.lastPlayedAt[category] or 0
  cooldown = (category == "warning") ? WARNING_COOLDOWN_MS : PER_BUTTON_COOLDOWN_MS
  if now - lastForCat < cooldown:
    return deny("category cooldown")

  if isQuietHours(now) and not state.longPressConfirmed:
    return deny("quiet hours, hold to play")

  if category == "warning" and not state.warningConfirmed:
    return deny("warning needs confirmation")

  if taperMode.enabled:
    extra = taperMultiplier(state.taperMode, now)
    if now - lastForCat < cooldown * extra:
      return deny("taper cooldown")

  return allow()

function playOnce(category, state):
  decision = canPlay(category, state, Date.now())
  if decision.denied:
    showStatus(decision.reason)
    return

  variant = pickVariant(category, state.lastVariant[category])
  state.lastVariant[category] = variant
  state.lastPlayedAt[category] = Date.now()
  state.lastGlobalPlayAt = Date.now()
  state.dailyPlayCount += 1
  persist(state)

  audio.replay(variant)

  if state.dailyPlayCount == DAILY_SOFT_CAP:
    softNudge("consider resting")

function pickVariant(category, last):
  pool = clipRegistry[category]
  if pool.length == 1: return pool[0]
  candidate = randomFrom(pool)
  while candidate == last: candidate = randomFrom(pool)
  return candidate
```

## 16. Interaction Flow

First launch. User opens app. Info screen appears with the rehab-first message and ethics statement. User taps "I understand." The flag is persisted. App moves to main screen.

Normal use. User taps a category. If allowed, the clip plays once. The button visually dims for 120 ms. The status bar updates to show the next-available time for that category. If denied, the status bar shows the reason in plain language for 2 seconds, then returns to "ready."

Warning use. User taps Warning. A small inline confirmation appears under the button: "Confirm warning call." User taps again within 3 seconds to play, or the prompt fades.

Quiet hours. Any button requires a 1 second long press. A small text under the button reads "hold to play."

Daily soft cap reached. Status bar dims; counter shows "8 / suggested 8, consider resting."

Daily hard cap reached. All buttons disabled. Single message: "rest period; the duckling benefits from quiet." Resets at local midnight.

Taper mode. Toggle in settings. From the day it is enabled, cooldowns scale up by 1.5x per day. On day 7, the app shows: "Thank you. Consider uninstalling now."

## 17. Recommendations for Sourcing or Generating Realistic Merganser Calls Ethically

This is the part to get right. Bad audio defeats the purpose.

Preferred sources, in order:

1. Recordings released under permissive licenses on Xeno-canto (xeno-canto.org) and the Macaulay Library (Cornell Lab of Ornithology). Filter for common merganser (Mergus merganser) and, where appropriate, red-breasted merganser (Mergus serrator). Use only juvenile or contact call recordings, not display or alarm calls, unless the category is "Warning."
2. Direct contact with a licensed wildlife rehabilitator who works with mergansers. Some will share recordings of healthy juveniles for rehab purposes.
3. Field recordings made by the developer at a respectful distance with a parabolic mic, near wild flocks, with no playback used to attract them. If this is not possible, skip it.

What to avoid:

- Generic stock-library "duck quacks." Mallards sound nothing like mergansers and using them risks confusing the duckling.
- AI-generated bird vocalizations. Current generators do not reliably produce species-accurate calls and may introduce artifacts a bird would react to in unpredictable ways.
- Heavily processed or reverberant clips. Keep them dry and natural.

Licensing checklist before bundling a clip:

- Confirm license allows redistribution in a free or paid app.
- Credit the recordist in the Info screen, even if not required.
- Keep a small `docs/sourcing.md` with one line per clip: file name, source URL, recordist, license, date downloaded.

Editing each clip:

- Trim to a single natural utterance.
- High-pass at 120 Hz to remove rumble.
- Light noise reduction only if needed; do not scrub the texture out of it.
- Peak normalize to -6 dBFS.
- Export to mono AAC at 128 kbps.

## 18. Development Roadmap (Solo Developer)

Day 1. Project setup. Expo blank template. Theme tokens. Empty MainScreen with five placeholder buttons. AsyncStorage wiring.

Day 2. Rules layer. Pure functions for cooldowns, daily caps, quiet hours, taper. Unit tests with Jest. No UI work.

Day 3. Audio layer. Bundle two placeholder clips. Preload at startup. Play once on button tap. Verify Android playback latency feels instant.

Day 4. UI polish. Status bar. Cooldown countdown. Warning confirmation. Quiet-hours long press. First-launch gate.

Day 5. Source and edit real audio clips. Replace placeholders. Write `sourcing.md` with licenses. Write `ethics.md`.

Day 6. Settings: output gain, taper toggle. Daily reset logic at local midnight. Manual end-to-end test outdoors with a real phone, in sunlight, with wet hands.

Day 7. Build a signed APK. Install on the carer's device. Step back. Add nothing.

After release: do not add features. If usage data could be collected, do not collect it. The app is finished.

## 19. Info Panel Content (Suggested Copy)

> If you have found a wild duckling, please contact a licensed wildlife rehabilitator before anything else. Trained rehabbers and species-appropriate housing give the bird its best chance.
>
> FlockCall is a stopgap. It plays a small set of natural merganser calls to reduce stress during short-term care, never to bond the duckling with a human. Keep your voice low, your movements slow, and your face out of view as much as possible.
>
> Use the app sparingly. Long silence is good. Outdoor time near water, with no playback, is better than any button in this app.
>
> Keep the speaker at arm's length from the bird. Never point it directly at the duckling from close range. If the bird startles or freezes, stop using the app and give it quiet space.
>
> When the duckling stabilizes, turn on Taper Mode. The app will become harder to use each day, by design.

## 20. Risks and What to Watch For

- Carer over-uses the app. Mitigation: hard daily cap, taper mode, copy that consistently frames silence as the goal.
- Audio is wrong species or wrong context. Mitigation: source carefully, label every clip, prefer juvenile contact calls.
- App becomes a social-media object. Mitigation: do not add sharing, do not add visuals beyond the five buttons, do not market.
- Imprinting risk from carer presence regardless of app. Mitigation: Info panel explicitly addresses this; app is not a substitute for rehab.

## 21. License and Distribution

Ship as a sideloadable APK and, if desired, on F-Droid. Avoid the Play Store unless the developer wants the review overhead. License the source code permissively (MIT or Apache 2.0). License the audio clips according to their individual sources, listed in `docs/sourcing.md`.

No telemetry. No update server. If a bug needs fixing, ship a new APK.

## 22. Done Definition

The app is done when:

- All five buttons play correct species-typical clips.
- All cooldowns and caps work as specified, verified by unit tests and one outdoor session.
- The first-launch screen leads with the rehabilitator-first message.
- Taper mode demonstrably increases cooldowns over a week.
- The APK installs and runs offline on a mid-range Android phone.

Stop there.
