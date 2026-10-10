# nUpdate homepage (mockup)

A static mockup of a homepage for nUpdate 5: plain HTML, one stylesheet and a small script, no framework and no build
step. Open `index.html` in a browser, or serve the folder:

```bash
python3 -m http.server --directory website 8000
```

- `index.html`: the homepage. Its code samples are the real API.
- `api.html`: the API reference, linked from the homepage.
- `imprint.html` and `privacy.html`: the legal notice (Impressum) and the privacy policy, in German with an English
  translation, linked from every footer. The privacy policy states that the site sets no cookies and loads nothing from
  other servers; keep it that way, or update the policy.
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

The fonts (Schibsted Grotesk, Instrument Sans, JetBrains Mono and Inter, all under the SIL Open Font License) are
served from `fonts/`, Latin and Latin Extended only, with their licenses; the pages load nothing from other servers,
which the privacy policy relies on.
