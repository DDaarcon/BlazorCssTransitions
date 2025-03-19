
/**
 * @param {HTMLElement} element
 */
export function ensureStylesWereApplied(element) {
    if (!element)
        return;

    // Force reflow to ensure styles were applied
    element.offsetHeight;
}