
/**
 * 
 * @param {HTMLElement} elementRef
 * @param {any} dotNetRef
 * @returns {ResizeObserver}
 */
export function listenForSizeChanges(element, dotNetRef) {
  if (!element)
    return new ResizeObserver(() => {});

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
      console.error('Error invoking .NET method.', error);
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
  if (observer)
    observer.disconnect();
}