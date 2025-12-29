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

    // Preserve insertion order
    const entries = Object.entries(data);

    const total = entries.reduce(
        (a, [, b]) => a + (Number.isFinite(b) ? b : 0),
        0
    );

    const finalCtx = canvas.getContext("2d");
    canvas.width = width;
    canvas.height = height;

    if (total <= 0) {
        finalCtx.clearRect(0, 0, width, height);
        return;
    }

    const tmp = document.createElement("canvas");
    tmp.width = lowRes;
    tmp.height = lowRes;
    const ctx = tmp.getContext("2d");

    const cx = lowRes / 2;
    const cy = lowRes / 2;
    const R = Math.max(0, cx - outerPadding);
    const r = Math.max(0, R * Math.min(Math.max(innerRatio, 0.05), 0.9));

    let start = -Math.PI / 2;

    for (let idx = 0; idx < entries.length; idx++) {
        const [, value] = entries[idx];
        const angle = (value / total) * Math.PI * 2;
        const end = start + angle;

        if (!isFinite(angle) || angle <= 0) {
            start = end;
            continue;
        }

        const path = new Path2D();
        path.arc(cx, cy, R, start, end, false);
        path.arc(cx, cy, r, end, start, true);

        const [pr, pg, pb] = GBA_PALETTE[idx % GBA_PALETTE.length];
        ctx.fillStyle = `rgb(${pr},${pg},${pb})`;
        ctx.fill(path, "evenodd");

        start = end;
    }

    if (dither) {
        applyDitherAndPalette(ctx, lowRes, lowRes);
    }

    finalCtx.imageSmoothingEnabled = false;
    finalCtx.drawImage(tmp, 0, 0, width, height);
}

// -------------------------------
// Legend
// -------------------------------
export function renderLegend(canvasId, data, options = {}) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const entries = Object.entries(data);

    const lineH = 22;
    const padX = 10;
    const padY = 10;

    const width = options.width ?? 256;
    const minHeight = options.height ?? 200;
    const neededHeight = padY * 2 + entries.length * lineH;
    const height = Math.max(minHeight, neededHeight);

    const tmp = document.createElement("canvas");
    tmp.width = width;
    tmp.height = height;
    const ctx = tmp.getContext("2d");
    ctx.imageSmoothingEnabled = false;

    ctx.font = "14px monospace";
    ctx.textAlign = "left";

    entries.forEach(([game], idx) => {
        const y = padY + idx * lineH;
        const [r, g, b] = GBA_PALETTE[idx % GBA_PALETTE.length];

        ctx.fillStyle = `rgb(${r},${g},${b})`;
        ctx.fillRect(padX, y, 16, 16);

        ctx.fillStyle = "white";
        ctx.fillText(game, padX + 25, y + 13);
    });

    if (options.dither) {
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
export function renderHistogram(canvasId, data, opts = {}) {
    const canvas = document.getElementById(canvasId);
    if (!canvas || !Array.isArray(data)) return;

    const ctx = canvas.getContext("2d");
    ctx.imageSmoothingEnabled = false;

    const W = canvas.width;
    const H = canvas.height;

    const padding = opts.padding ?? 40;

    // --------------------------------------------------
    // Normalise data (ARRAY -> grouped by year)
    // --------------------------------------------------
    const byYear = {};
    data.forEach(e => {
        if (!byYear[e.year]) byYear[e.year] = [];
        byYear[e.year].push({
            month: e.month,
            quoteCount: e.quoteCount
        });
    });

    const years = Object.keys(byYear).sort();
    if (years.length === 0) return;

    // Ensure months are ordered
    years.forEach(y =>
        byYear[y].sort((a, b) => a.month - b.month)
    );

    // --------------------------------------------------
    // Legend sizing
    // --------------------------------------------------
    const legendLineH = 20;
    const legendHeight = years.length * legendLineH + 10;
    const topPadding = padding + legendHeight;

    // --------------------------------------------------
    // Max value
    // --------------------------------------------------
    const maxValue = Math.max(
        ...data.map(e => e.quoteCount),
        1
    );

    ctx.clearRect(0, 0, W, H);

    // --------------------------------------------------
    // X-axis
    // --------------------------------------------------
    ctx.strokeStyle = "#ffffff";
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.moveTo(padding, H - padding);
    ctx.lineTo(W - padding, H - padding);
    ctx.stroke();

    const months = byYear[years[0]].map(e => e.month);
    const groupWidth = (W - padding * 2) / months.length;
    const barWidth = groupWidth / years.length;

    const monthNames = [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    ];

    // --------------------------------------------------
    // Colours: reverse GBA palette
    // --------------------------------------------------
    const reversedPalette = [...GBA_PALETTE]
        .reverse()
        .filter((_, i) => i % 2 === 0);

    // --------------------------------------------------
    // Bars
    // --------------------------------------------------
    months.forEach((month, mIdx) => {
        years.forEach((year, yIdx) => {
            const entry = byYear[year][mIdx];
            if (!entry) return;

            const barHeight =
                (entry.quoteCount / maxValue) *
                (H - topPadding - padding);

            const x =
                padding +
                mIdx * groupWidth +
                yIdx * barWidth;

            const y = H - padding - barHeight;

            const [r, g, b] =
                reversedPalette[yIdx % reversedPalette.length];

            ctx.fillStyle = `rgb(${r},${g},${b})`;
            ctx.fillRect(x, y, barWidth - 1, barHeight);

            if (opts.retroOutline) {
                ctx.strokeStyle = "#000";
                ctx.lineWidth = 1;
                ctx.strokeRect(x, y, barWidth - 1, barHeight);
            }
        });
    });

    // --------------------------------------------------
    // Month labels
    // --------------------------------------------------
    ctx.fillStyle = "#ffffff";
    ctx.font = "12px monospace";
    ctx.textAlign = "center";

    months.forEach((month, i) => {
        const x = padding + i * groupWidth + groupWidth / 2;
        ctx.fillText(
            monthNames[month - 1],
            x,
            H - padding + 15
        );
    });

    // --------------------------------------------------
    // Legend
    // --------------------------------------------------
    let ly = padding;
    ctx.font = "14px monospace";
    ctx.textAlign = "left";

    years.forEach((year, i) => {
        const [r, g, b] =
            reversedPalette[i % reversedPalette.length];

        ctx.fillStyle = `rgb(${r},${g},${b})`;
        ctx.fillRect(padding, ly, 12, 12);

        ctx.strokeStyle = "#000";
        ctx.strokeRect(padding, ly, 12, 12);

        ctx.fillStyle = "#ffffff";
        ctx.fillText(year, padding + 20, ly + 11);

        ly += legendLineH;
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