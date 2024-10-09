
/**
 * 
 * @param {HTMLElement} elementRef
 * @param {any} dotNetRef
 * @returns {ResizeObserver}
 */
export function listenForSizeChanges(element, dotNetRef) {

  const observer = new ResizeObserver(() => {

    /**
     * @type {import("./scripts").DOMScrollSize}
     */
    const offsetRect = {
      width: element.scrollWidth,
      height: element.scrollHeight,
      left: element.scrollLeft,
      top: element.scrollTop
    }

    try {
      dotNetRef.invokeMethodAsync('Invoke', offsetRect);
    }
    catch (error) {
      debugger
    }
  });

  observer.observe(element);

  return observer;
}

/**
 * 
 * @param {ResizeObserver} observer
 */
export function stopListeningFoSizeChanges(observer) {
  observer.disconnect();
}