# CapEx26 poster and pitch (Team UG_ICT_P01)

Submission files for Task 5: an A1 portrait poster and a one-slide elevator pitch.

| File | What it is |
|---|---|
| `UG_ICT_P01_poster.pdf` | A1 poster to submit |
| `UG_ICT_P01_pitch.pdf` | One-slide pitch to submit |
| `poster.html`, `pitch.html` | The source. Edit these, then rebuild |
| `pptx/` | PowerPoint copies for anyone who prefers dragging things around (in the repo, not the zip; see below) |
| `images/`, `logos/` | Screenshots and logos used by both |
| `build.sh` | Rebuilds both PDFs from the HTML |

## Editing the HTML (the master copy)

1. Open `poster.html` or `pitch.html` in any text editor. Each section is labelled with a comment (`<!-- 01 PROBLEM -->` etc.), and the wording sits between the tags.
2. Open the same file in Chrome or Edge to preview. Refresh after each save.
3. Rebuild the PDFs:

   ```bash
   bash build.sh
   ```

   Run it from this folder (in the repo that's `docs/poster/`). Needs Chrome or Edge installed. On Windows run it from Git Bash.
4. Check `preview_poster.png` to make sure nothing spills off the bottom of the page.

Keep these in place (they are marked in the brief):
- Swinburne logo top-right (`logos/swinburne.png`, never stretched)
- `UG_ICT_P01` code square bottom-right
- All team members, supervisor (Naveed Ali) and client (Harry Macheda)
- Text no smaller than about 22pt on the poster, as requested by our supervisor

## Editing the PowerPoint copies

`pptx/UG_ICT_P01_poster.pptx` (A1 page size) and `pptx/UG_ICT_P01_pitch.pptx` have editable text boxes. Export with File > Save As > PDF. If you edit these, say so in the team chat, because the HTML and the PowerPoint copies don't sync with each other.
