window.renderWordCloud = (canvasId, items) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const placed = [];
    const margin = 4;
    const gridStep = 4;

    function rectsIntersect(a, b) {
        return !(a.x + a.w < b.x || a.x > b.x + b.w || a.y + a.h < b.y || a.y > b.y + b.h);
    }
    function intersects(x, y, w, h) {
        const rect = { x, y, w, h };
        for (const p of placed) {
            if (rectsIntersect(rect, p)) return true;
        }
        return false;
    }

    for (const item of items) {
        // I HATE SCRIPTING LANGUAGES, I HATE THE ANTICHRIST
        const text = item.word ?? item.Word ?? "";
        const fontSize = item.fontSize ?? item.FontSize ?? item.fontSizePx ?? 16;
        const colour = item.colour ?? item.Colour ?? "#fff";

        ctx.font = `${fontSize}px 'Press Start 2P', Arial`;

        const metrics = ctx.measureText(text);
        const width = metrics.width;
        // Try to get real ascent/descent; fall back to proportions if unavailable
        const ascent = (metrics.actualBoundingBoxAscent !== undefined) ? metrics.actualBoundingBoxAscent : fontSize * 0.8;
        const descent = (metrics.actualBoundingBoxDescent !== undefined) ? metrics.actualBoundingBoxDescent : fontSize * 0.25;
        const height = ascent + descent;

        let placedOK = false;

        let startLeft = Math.floor(Math.random() * Math.max(1, (canvas.width - width - margin)));
        let startBaseline = Math.floor(Math.random() * Math.max(1, (canvas.height - margin - descent)));
        startBaseline = Math.max(Math.ceil(ascent + margin), Math.min(canvas.height - Math.ceil(descent + margin), startBaseline));

        const maxRadius = 80; // Completely arbitrary, but it works, might need to test it in different canvas sizes
        for (let radius = 0; radius < maxRadius && !placedOK; radius++) {
            for (let dx = -radius; dx <= radius && !placedOK; dx++) {
                for (let dy = -radius; dy <= radius && !placedOK; dy++) {
                    let left = startLeft + dx * gridStep;
                    let baseline = startBaseline + dy * gridStep;

                    // Boundaries / Overlapping prevention
                    left = Math.max(margin, Math.min(canvas.width - width - margin, left));
                    baseline = Math.max(Math.ceil(ascent + margin), Math.min(canvas.height - Math.ceil(descent + margin), baseline));

                    // """Collision detection"""
                    const top = Math.floor(baseline - ascent);
                    const rect = { x: left, y: top, w: Math.ceil(width), h: Math.ceil(height) };

                    if (!intersects(rect.x, rect.y, rect.w, rect.h)) {
                        placed.push(rect);

                        ctx.fillStyle = colour;
                        ctx.font = `${fontSize}px 'Press Start 2P', Arial`;
                        ctx.fillText(text, left, baseline);

                        placedOK = true;
                    }
                }
            }
        }

        if (!placedOK) {
            console.log(`This word is TEW BEEG: "${text}"`);
        }
    }
};