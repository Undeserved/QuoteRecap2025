// -------------------------------
// Palette + Dither
// -------------------------------
const GBA_PALETTE = [
    [0, 0, 0], [31, 0, 0], [0, 31, 0], [0, 0, 31],
    [31, 31, 0], [31, 0, 31], [0, 31, 31],
    [31, 31, 31], [15, 15, 15], [31, 15, 0],
    [0, 15, 31], [15, 0, 31], [31, 0, 15],
    [0, 31, 15], [15, 31, 0]
].map(rgb => rgb.map(v => Math.round(v * 8)));

function matchPalette(r, g, b) {
    let best = 1e12;
    let chosen = [0, 0, 0];
    for (const [pr, pg, pb] of GBA_PALETTE) {
        const dist = (r - pr) ** 2 + (g - pg) ** 2 + (b - pb) ** 2;
        if (dist < best) {
            best = dist;
            chosen = [pr, pg, pb];
        }
    }
    return chosen;
}

const BAYER_4x4 = [
    [0, 8, 2, 10],
    [12, 4, 14, 6],
    [3, 11, 1, 9],
    [15, 7, 13, 5]
];

function applyDitherAndPalette(ctx, w, h) {
    const img = ctx.getImageData(0, 0, w, h);
    const d = img.data;

    for (let y = 0; y < h; y++) {
        for (let x = 0; x < w; x++) {
            const i = (y * w + x) * 4;
            const r = d[i], g = d[i + 1], b = d[i + 2];
            const threshold = BAYER_4x4[y % 4][x % 4];
            const adjR = r + threshold - 8;
            const adjG = g + threshold - 8;
            const adjB = b + threshold - 8;
            const [nr, ng, nb] = matchPalette(adjR, adjG, adjB);
            d[i] = nr; d[i + 1] = ng; d[i + 2] = nb;
        }
    }
    ctx.putImageData(img, 0, 0);
}

// -------------------------------
// Ring Pie Chart
// -------------------------------
export function renderRingPieChart(canvasId, data, options = {}) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const {
        width = 256,
        height = 256,
        lowRes = 128,
        outerPadding = 4,
        innerRatio = 0.45,
        dither = true
    } = options;

    const tmp = document.createElement("canvas");
    tmp.width = lowRes;
    tmp.height = lowRes;
    const ctx = tmp.getContext("2d");

    const total = Object.values(data).reduce((a, b) => a + (Number.isFinite(b) ? b : 0), 0);
    if (total <= 0) {
        const finalCtx = canvas.getContext("2d");
        canvas.width = width;
        canvas.height = height;
        finalCtx.clearRect(0, 0, width, height);
        return;
    }

    const cx = lowRes / 2;
    const cy = lowRes / 2;
    const R = Math.max(0, cx - outerPadding);
    const r = Math.max(0, R * Math.min(Math.max(innerRatio, 0.05), 0.9));

    let start = -Math.PI / 2;
    const entries = Object.entries(data);

    for (let idx = 0; idx < entries.length; idx++) {
        const [game, value] = entries[idx];
        const angle = (value / total) * Math.PI * 2;
        const end = start + angle;
        if (!isFinite(angle) || angle <= 0) {
            start = end;
            continue;
        }

        const path = new Path2D();
        // outer arc (clockwise)
        path.arc(cx, cy, R, start, end, false);
        // inner arc (counter-clockwise)
        path.arc(cx, cy, r, end, start, true);

        const [pr, pg, pb] = GBA_PALETTE[idx % GBA_PALETTE.length];
        ctx.fillStyle = `rgb(${pr},${pg},${pb})`;
        ctx.fill(path, "evenodd");

        start = end;
    }

    if (dither) {
        applyDitherAndPalette(ctx, lowRes, lowRes);
    }

    // Upscale
    const finalCtx = canvas.getContext("2d");
    canvas.width = width;
    canvas.height = height;
    finalCtx.imageSmoothingEnabled = false;
    finalCtx.drawImage(tmp, 0, 0, width, height);
}

// -------------------------------
// Legend
// -------------------------------
export function renderLegend(canvasId, data, options = {}) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const { width = 256, height = 200, dither = false } = options;

    const tmp = document.createElement("canvas");
    tmp.width = width;
    tmp.height = height;
    const ctx = tmp.getContext("2d");
    ctx.imageSmoothingEnabled = false;

    ctx.font = "14px monospace";
    ctx.fillStyle = "white";

    const entries = Object.entries(data);
    const lineH = 22;
    const padX = 10;
    const padY = 10;

    entries.forEach(([game], idx) => {
        const y = padY + idx * lineH;
        const [r, g, b] = GBA_PALETTE[idx % GBA_PALETTE.length];
        ctx.fillStyle = `rgb(${r},${g},${b})`;
        ctx.fillRect(padX, y, 16, 16);

        ctx.fillStyle = "white";
        ctx.fillText(game, padX + 25, y + 13);
    });

    if (dither) {
        applyDitherAndPalette(ctx, width, height);
    }

    const finalCtx = canvas.getContext("2d");
    canvas.width = width;
    canvas.height = height;
    finalCtx.imageSmoothingEnabled = false;
    finalCtx.drawImage(tmp, 0, 0, width, height);
}

//-------------------------------
// Histogram
//-------------------------------

export function renderHistogram(canvasId, entries, opts) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const ctx = canvas.getContext("2d");
    const W = opts.width;
    const H = opts.height;

    canvas.width = W;
    canvas.height = H;

    ctx.clearRect(0, 0, W, H);

    const padding = opts.padding ?? 40;
    const barGap = opts.barGap ?? 4;

    // ---- Determine months & years ----
    const months = [...new Set(entries.map(e => e.month))].sort((a, b) => a - b);
    const years = [...new Set(entries.map(e => e.year))].sort((a, b) => a - b);

    // ---- Year & Colour map ----
    const palette = ["#E43F5A", "#FF9F1C", "#2EC4B6", "#1B98E0", "#9D4EDD"];
    const yearColor = {};
    years.forEach((y, i) => yearColor[y] = palette[i % palette.length]);

    const maxValue = Math.max(...entries.map(e => e.quoteCount), 1);

    const usableWidth = W - padding * 2;
    const columnWidth = usableWidth / months.length;

    // -------------------------
    // Draw Bars
    // -------------------------
    months.forEach((month, mIndex) => {
        const xBase = padding + mIndex * columnWidth;

        years.forEach((year, yIndex) => {
            const e = entries.find(e => e.month === month && e.year === year);
            if (!e) return;

            const barHeight = (e.quoteCount / maxValue) * (H - padding * 2);
            const bw = ((columnWidth - barGap) / years.length);
            const x = xBase + yIndex * bw;

            ctx.fillStyle = yearColor[year];
            ctx.fillRect(x, H - padding - barHeight, bw, barHeight);

            if (opts.retroOutline) {
                ctx.strokeStyle = "#000";
                ctx.lineWidth = 1;
                ctx.strokeRect(x, H - padding - barHeight, bw, barHeight);
            }
        });

        // ---- Month Label ----
        const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        ctx.fillStyle = "white";
        ctx.font = "14px monospace";
        ctx.textAlign = "center";
        ctx.fillText(monthNames[month - 1], xBase + columnWidth / 2, H - padding + 15);
    });

    // -------------------------
    // X-axis line
    // -------------------------
    ctx.strokeStyle = "white";
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.moveTo(padding, H - padding);
    ctx.lineTo(W - padding, H - padding);
    ctx.stroke();

    // -------------------------
    // Legend (top-left)
    // -------------------------
    let ly = padding - 20;

    years.forEach(year => {
        ctx.fillStyle = yearColor[year];
        ctx.fillRect(padding, ly, 12, 12);

        ctx.strokeStyle = "#000";
        ctx.strokeRect(padding, ly, 12, 12);

        ctx.fillStyle = "white";
        ctx.font = "14px monospace";
        ctx.textAlign = "left";
        ctx.fillText(year, padding + 18, ly + 11);

        ly += 20;
    });
}


//-------------------------------
// Heatmap
//-------------------------------
export function renderHeatmap(canvasId, entries, opts = {}) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const ctx = canvas.getContext("2d");

    const W = opts.width ?? 600;
    const H = opts.height ?? 350;

    canvas.width = W;
    canvas.height = H;

    const paddingLeft = opts.paddingLeft ?? 60;
    const paddingTop = opts.paddingTop ?? 40;
    const paddingBottom = opts.paddingBottom ?? 40;
    const paddingRight = opts.paddingRight ?? 20;

    ctx.clearRect(0, 0, W, H);

    const hours = [...new Set(entries.map(e => e.hour))].sort((a, b) => a - b);
    const weekdays = [...new Set(entries.map(e => e.weekday))].sort((a, b) => a - b);

    const weekdayNames = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    // -----------------------------
    // Colour scale
    // -----------------------------
    const maxCount = Math.max(...entries.map(e => e.count), 1);

    function colourForValue(v) {
        const t = v / maxCount;
        const r = Math.round(255 * t);
        const g = Math.round(80 * (1 - t));
        const b = Math.round(80 * (1 - t));
        return `rgb(${r},${g},${b})`;
    }

    // -----------------------------
    // Cell size
    // -----------------------------
    const gridW = W - paddingLeft - paddingRight;
    const gridH = H - paddingTop - paddingBottom;

    const cellW = gridW / hours.length;
    const cellH = gridH / weekdays.length;

    // -----------------------------
    // Draw Weekday Y Axis
    // -----------------------------
    ctx.font = "14px 'Press Start 2P', monospace";
    ctx.fillStyle = "#ffffff";
    ctx.textAlign = "right";

    weekdays.forEach((wd, row) => {
        const y = paddingTop + row * cellH + cellH / 2 + 5;
        ctx.fillText(weekdayNames[wd - 1], paddingLeft - 10, y);
    });

    // -----------------------------
    // Draw Hour X Axis
    // -----------------------------
    ctx.textAlign = "center";
    ctx.font = "12px 'Press Start 2P', monospace";

    hours.forEach((hr, col) => {
        const x = paddingLeft + col * cellW + cellW / 2;

        ctx.fillText(hr.toString().padStart(2, "0"), x, paddingTop - 10);
    });

    // -----------------------------
    // Draw heatmap cells
    // -----------------------------
    weekdays.forEach((wd, row) => {
        hours.forEach((hr, col) => {

            const cell = entries.find(e => e.hour === hr && e.weekday === wd);
            const count = cell ? cell.count : 0;

            const x = paddingLeft + col * cellW;
            const y = paddingTop + row * cellH;

            ctx.fillStyle = colourForValue(count);
            ctx.fillRect(x, y, cellW, cellH);

            if (opts.retroGrid ?? true) {
                ctx.strokeStyle = "#000";
                ctx.lineWidth = 1;
                ctx.strokeRect(x, y, cellW, cellH);
            }
        });
    });
}