
/**
 * 
 * @param {string} propertyName
 * @returns {string}
 */
export function readRootProperty(propertyName) {
  const root = document.querySelector(':root');

  return getComputedStyle(root).getPropertyValue(propertyName);
}