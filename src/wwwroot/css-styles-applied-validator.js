
/**
 * @param {HTMLElement} element
 */
export function ensureStylesWereApplied(element) {
    if (!element)
        return;

    // Force reflow to ensure styles apply before adding the class
    element.offsetHeight;
}