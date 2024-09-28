// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

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