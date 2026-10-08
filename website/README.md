# nUpdate homepage (mockup)

A static mockup of a homepage for nUpdate 5: plain HTML, one stylesheet and a small script, no framework and no build
step. Open `index.html` in a browser, or serve the folder:

```bash
python3 -m http.server --directory website 8000
```

- `index.html`: the homepage. Its code samples are the real API.
- `api.html`: the API reference, linked from the homepage.
- `styles.css`: colours as tokens with a dark variant that follows the system setting, and the animations, which stop
  under "reduce motion".
- `main.js`: the installer window cycling through Windows, Linux and macOS, the platform picker (the real rule:
  runtime identifier, then operating system, then `any`), tabs, copy buttons, scroll reveals and a small highlighter.
- `img/`: a copy of `assets/nupdate-icon.png` and the icon of the sample app Aurora.

`api.html` is plain HTML: the public types of nUpdate 5 with their real signatures and an example for each type and the
main calls, grouped like the packages, on one page. The examples compile against the libraries. The list on the left jumps to a type (each has an anchor, such as `api.html#UpdateFlow`), marks the one in view
and filters the members. Update it when the public API changes.

The Administration screens are rebuilt in HTML in a Windows 11 frame, after the real windows (texts, layout,
colours); they follow light and dark mode and scale down on narrow screens.

Before going live, self-host the three fonts (Schibsted Grotesk, Instrument Sans, JetBrains Mono, all under the SIL
Open Font License) instead of loading them from Google Fonts.
