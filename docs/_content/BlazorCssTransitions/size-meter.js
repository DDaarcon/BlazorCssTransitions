
/**
 * @param {HTMLElement} element
 * @returns {DOMRect}
 */
export function measureElement(element) {
    return element.getBoundingClientRect();
}



/**
 * @typedef {Object} DOMScrollSize
 * @property {number} width
 * @property {number} height
 * @property {number} left
 * @property {number} top
 */

/**
 * @param {HTMLElement} element
 * @returns {DOMScrollSize}
 */
export async function measureElementScroll(element) {
    return {
        width: element.scrollWidth,
        height: element.scrollHeight,
        left: element.scrollLeft,
        top: element.scrollTop
    }
}