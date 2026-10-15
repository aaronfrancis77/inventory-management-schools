#!/usr/bin/env bash
# Renders poster.html and pitch.html to PDF (plus PNG previews) with headless Chrome/Edge.
# Works on Windows (Git Bash), macOS and Linux. Override the browser with BROWSER=/path/to/chrome.
set -e
cd "$(dirname "$0")"

if [ -z "$BROWSER" ]; then
  for b in \
    "/c/Program Files (x86)/Microsoft/Edge/Application/msedge.exe" \
    "/c/Program Files/Google/Chrome/Application/chrome.exe" \
    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome" \
    "/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge" \
    "$(command -v google-chrome 2>/dev/null)" "$(command -v chromium 2>/dev/null)" "$(command -v chromium-browser 2>/dev/null)"; do
    if [ -n "$b" ] && [ -x "$b" ]; then BROWSER="$b"; break; fi
  done
fi
[ -n "$BROWSER" ] || { echo "No Chrome/Edge found. Set BROWSER=/path/to/chrome"; exit 1; }

# Windows browsers need Windows-style paths; elsewhere use the plain directory.
if command -v cygpath >/dev/null 2>&1; then
  OUT="$(cygpath -w "$PWD")\\"; URL="file:///$(cygpath -m "$PWD")/"
else
  OUT="$PWD/"; URL="file://$PWD/"
fi

run() { "$BROWSER" --headless=new --disable-gpu --hide-scrollbars --no-pdf-header-footer --virtual-time-budget=4000 "$@" 2>/dev/null; }

for name in poster pitch; do
  run --print-to-pdf="${OUT}UG_ICT_P01_$name.pdf" "${URL}$name.html"
done
run --window-size=2245,3179 --screenshot="${OUT}preview_poster.png" "${URL}poster.html"
run --window-size=1280,720 --force-device-scale-factor=1.5 --screenshot="${OUT}preview_pitch.png" "${URL}pitch.html"
echo "Built UG_ICT_P01_poster.pdf and UG_ICT_P01_pitch.pdf"
