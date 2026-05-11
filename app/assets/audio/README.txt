FlockCall audio folder
======================

The app looks for these files at startup. If none are present, it falls back to clearly-labeled synthesized placeholder tones (do not use placeholders with a real bird).

Expected filenames (drop in any subset; the app uses what it finds):

  contact_01.m4a
  contact_02.m4a
  gather_01.m4a
  gather_02.m4a
  feeding_01.m4a
  feeding_02.m4a
  warning_01.m4a
  location_01.m4a
  location_02.m4a

Recommended encoding:
  - AAC in .m4a container, mono, 44.1 kHz, 96 to 128 kbps
  - Single utterance, 0.8 to 2.5 seconds
  - Peak normalized to -6 dBFS, no loudness maximization
  - Dry (no added reverb), high-passed at ~120 Hz

Sourcing:
  - Xeno-canto (xeno-canto.org) and Macaulay Library (Cornell Lab of Ornithology)
  - Filter by species: Mergus merganser (preferred) or Mergus serrator
  - Prefer juvenile / contact / location calls; use display or alarm calls only for the Warning category
  - Verify the license allows redistribution in your build
  - Record provenance in ../docs/sourcing.md (file name, source URL, recordist, license, date)

Do NOT use:
  - Generic stock "duck quack" clips (wrong species)
  - AI-generated bird calls (unreliable acoustic accuracy)
  - Heavily processed or reverberant clips
