export class AjaxHelper {

    constructor() {

    }

    async postRequest<T>(Url: string, Data?: any): Promise<T> {
        const response = await fetch(Url, {
            method: "POST",
            mode: 'cors',
            cache: "no-cache",
            headers: {
                'Content-Type': 'application/json'
            },
            body: (Data ? JSON.stringify(Data) : "")
        });
        return await response.json();
    }

    async getRequest<T>(Url: string, Data?: Record<string, (string | number | boolean)>): Promise<T> {
        var urlWithParams = Url;
        if (Data) {
            let params = new Array<string>();
            for(let key in Data) {
                params.push(encodeURI(key)+"="+encodeURI(Data[key].toString()))
            }
            urlWithParams += "?" + params.join("&");
        }
        const response = await fetch(urlWithParams, {
            method: "GET",
            mode: 'cors',
            cache: "no-cache"
        });
        return await response.json();
    }
}