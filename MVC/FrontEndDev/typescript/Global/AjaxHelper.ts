export class AjaxHelper {

    constructor() {

    }

    /*
     * Performs a POST on the given url, passing the given data (if any) as the Body post
     * @param Url - Url of the API to hit
     * @param Data - The JSON object the post as the body parameters
     * @returns response parsed as T Type
    */
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

     /*
     * Performs a GET on the given url, passing any data records as query parameters, and parsing the response as T object type
     * @param Url - Url of the API to hit
     * @param Data - Key (string) value (string, number, or boolean) of any parameters to include in the call, these are parsed as URL variables    
     * @returns response parsed as T Type
    */
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

    /*
     * Runs the given promise calls in order against the provided array of values, waiting for each item to finish it's promise before executing the next
     * @param values - Array of values that will execute in the funcToRun in order
     * @param funcToRun - Function that takes the single value and returns a Promise<TPromise> of the asynchronous call (ex postRequest or getRequest of this helper)
     * @param funcAfterEachRun - Optional function that receives the Promise's result, if you return false then aborts the rest of the calls, nothing / true continues
     * @param funcWhenFinished - Optional function that runs once every item is executed.
    */
    public runInSequence<T, TPromise>(values : Array<T>, funcToRun : (value : T) => Promise<TPromise>, funcAfterEachRun? : (value : TPromise) => boolean | any | undefined, funcWhenFinished? : () => any | undefined) : void {
        if(values.length > 0) {
            var value = values[0];
            values = values.splice(1);
            funcToRun(value).then((tPromise : TPromise) => {
                if(typeof funcAfterEachRun !== undefined && funcAfterEachRun != null){
                    var result = funcAfterEachRun(tPromise);
                    if(typeof result === "boolean" && !(result as boolean)) {
                        // Abort
                        return;
                    }
                }
                this.runInSequence(values, funcToRun, funcAfterEachRun, funcWhenFinished);
            });
        } else {
            if(typeof funcWhenFinished !== undefined){
                funcWhenFinished!!();
            }
        }
        return;
    }
}