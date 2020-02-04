(function () {
    window.blazorLocalStorage = {
        getItem: key => key in localStorage ? JSON.parse(localStorage[key]) : null,
        setItem: (key, value) => { localStorage[key] = JSON.stringify(value); },
        removeItem: key => { delete localStorage[key]; },
        containsKey: key => { localStorage.getItem(key) === null; },
        clear: () => { localStorage.clear();  }
    };
})();