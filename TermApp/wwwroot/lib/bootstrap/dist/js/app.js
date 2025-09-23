export function clearListCache() {
    try {
        localStorage.removeItem("termListFilter");
        localStorage.removeItem("termListPage");
        sessionStorage.removeItem("termListSelection");
        if (window.caches?.keys) {
            caches.keys().then(keys => keys.forEach(k => caches.delete(k)));
        }
    } catch { }
}
