export class BrowserDataHelper {

    constructor() {

    }

    // Query String Helpers
    getQueryParam(name: string, url : string = window.location.href) : string | null {
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)', 'i'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    getQueryParamTyped<T>(name: string, isJson: boolean, url : string = window.location.href) : T | null {
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)', 'i'),
            results = regex.exec(url);
        if (!results) {
            return null;
        }
        var valueToParse = !results[2] ? "" : results[2];
        var value = decodeURIComponent(valueToParse.replace(/\+/g, ' '));
        try {
            return isJson ? JSON.parse(value) as T : value as T;
        }   catch(err) {
            console.log(err);
            return null;
        }
    }

    // TODO, make getQueryParams (key: string) : Array<string> | null

    // TODO, make getQueryParamsTyped<T>(key: string) : Array<T> | null

    // TODO, make setQueryParams(key: string, value: string | Array<string> | number | Array<number>, queryString: string = window.location.search) : string

    // TODO, make removeQueryParams(key: string, queryString: string = window.location.search) : string

    // Cookie Storage
    setCookie(name: string, value: string,  expirationDays: number = 0, path: string = "/", domain: string = "", secure: boolean = false, sameSite: boolean = false) {
        var maxAgeVal = 60*60*24*(expirationDays > 0 ? expirationDays : 365);
        var domainSegment = domain ? "; domain="+domain : "";
        var secureSegment = secure ? ";secure" : "";
        var sameSiteSegment = sameSite ? ";samesite" : "";
        document.cookie = encodeURIComponent(name)+"="+encodeURIComponent(value)+"; max-age="+maxAgeVal+"; path="+path+domainSegment+secureSegment+sameSiteSegment+";";
    }

    setCookieTyped<T>(name: string, value: T, isJson: boolean, expirationDays: number = 0, path: string = "/", domain: string = "", secure: boolean = false, sameSite: boolean = false) {
        var maxAgeVal = 60*60*24*(expirationDays > 0 ? expirationDays : 365);
        var domainSegment = domain ? "; domain="+domain : "";
        var secureSegment = secure ? ";secure" : "";
        var sameSiteSegment = sameSite ? ";samesite" : "";
        var valueToSet = isJson ? JSON.stringify(value) : value as string;
        document.cookie = encodeURIComponent(name)+"="+encodeURIComponent(valueToSet)+"; max-age="+maxAgeVal+"; path="+path+domainSegment+secureSegment+sameSiteSegment+";";;
    }

    getCookie(name: string) : string | null {
        if(document.cookie[name]) {
            return decodeURIComponent(document.cookie[name]);
        } else {
            return null;
        }
    }

    getCookieTyped<T>(name: string, isJson: boolean) : T | null {
        if(document.cookie[name]) {
            var value = decodeURIComponent(document.cookie[name]);
            return isJson ? JSON.parse(value) as T : value as T;
        } else {
            return null;
        }
    }

    removeCookie(name: string, path: string = "/") {
        document.cookie = encodeURIComponent(name)+"; max-age=-1; path="+path+";";
    }

    // Session Storage
    setSessionStorage(name: string, value: string) {
        window.sessionStorage.setItem(name, value);
    }

    setSessionStorageTyped<T>(name: string, value: T, isJson: boolean) {
        window.sessionStorage.setItem(name, isJson ? JSON.stringify(value) : value as string);
    }

    getSessionStorage(name: string) : string | null {
        return window.sessionStorage.getItem(name);
    }

    getSessionStorageTyped<T>(name: string, isJson: boolean) : T | null {
        var value = window.sessionStorage.getItem(name);
        if(value) {
            return isJson ? JSON.parse(value) as T : value as T;
        }
        return null;
    }

    removeSessionStorage(name: string) {
        window.sessionStorage.removeItem(name);
    }

    // Local storage, with expiration custom logic    
    setLocalStorage(name: string, value: string, expirationDays: number = 0){
        window.localStorage.setItem(name, value);

        // Custom Expiration Logic
        if(expirationDays > 0) {
            var expDate = new Date(new Date().getDate()+expirationDays);
            window.localStorage.setItem(name+"-expiration", expDate.toUTCString());
        }
    }

    setLocalStorageTyped<T>(name: string, value: T, isJson: boolean, expirationDays: number = 0) {
        window.localStorage.setItem(name, isJson ? JSON.stringify(value) : value as string);

        // Custom Expiration Logic
        if(expirationDays > 0) {
            var expDate = new Date(new Date().getDate()+expirationDays);
            window.localStorage.setItem(name+"-expiration", expDate.toUTCString());
        }
    }

    getLocalStorage(name: string) : string | null {
        var value = window.localStorage.getItem(name);
        if(value) {
            var exp = window.localStorage.getItem(name+"-expiration");
            if(exp){
                var expDate = new Date(Date.parse(exp));
                if(expDate && expDate < new Date()){
                    // Expired
                    window.localStorage.removeItem(name);
                    window.localStorage.removeItem(name+"-expiration");
                    return null;
                }
            }
            return value;
        } 
        return null;
    }

    getLocalStorageTyped<T>(name: string, isJson: boolean) : T | null {
        var value = window.localStorage.getItem(name);
        if(value) {
            var exp = window.localStorage.getItem(name+"-expiration");
            if(exp){
                var expDate = new Date(Date.parse(exp));
                if(expDate && expDate < new Date()){
                    // Expired
                    window.localStorage.removeItem(name);
                    window.localStorage.removeItem(name+"-expiration");
                    return null;
                }
            }
            return isJson ? JSON.parse(value) as T : value as T ;
        } 
        return null;
    }

    removeLocalStorage(name: string) {
        window.localStorage.removeItem(name);
    }

}