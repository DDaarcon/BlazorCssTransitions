
/**
 * @param {HTMLElement} element
 */
export async function ensureStylesWereApplied(element) {
    if (!element)
        return;

    // Force reflow to ensure styles were applied
    element.offsetHeight;

    await waitForNextRenderFrame();
}

function waitForNextRenderFrame() {
    return new Promise(resolve => {
        const fallbackTimeoutId = setTimeout(resolveOnce, 100); // Fallback to ensure we don't wait indefinitely
        const animationFrameRequestId = requestAnimationFrame(resolveOnce);

        function resolveOnce() {
            clearTimeout(fallbackTimeoutId);
            cancelAnimationFrame(animationFrameRequestId);

            resolve();
        }
    });
}