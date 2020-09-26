import * as http from "http";

/**
 * Api is a generic REST Api handler. Set your API url first.
 */
export class Api {
  baseUrl: string = "http://localhost:8043";
  servicesUrl: string = this.baseUrl + "/api/Status";

  get<T>(endpoint: string, ... params : {key : string, value : any }[]) : Promise<T> {

    return new Promise<T>((resolve : (response : T) => void, reject : (reason: any) => void) => {

      let queryString = "";

      if (params.length) {

        let argsArray = [];

        params.forEach(a => {

          if (queryString.length) {
            queryString += `&${ a.key }=${ a.value }`;
          }
          else {
            queryString = `${ a.key }=${ a.value }`;
          }
        });
      }
  
      http.get(this.servicesUrl + '/' + endpoint + (queryString ? "?" + queryString : ""), (res) => {

        const { statusCode } = res;
        const contentType = res.headers['content-type'];
        let error;

        if (statusCode !== 200) {
          error = new Error('Request Failed.\n' + `Status Code: ${statusCode}`);
        } 
        else if (!/^application\/json/.test(contentType)) {
          error = new Error('Invalid content-type.\n' + `Expected application/json but received ${contentType}`);
        }

        if (error) {

          reject(error.message);
          // Consume response data to free up memory
          res.resume();

          return;
        }
      
        res.setEncoding('utf8');
        let rawData = '';

        res.on('data', (chunk) => { rawData += chunk; });
        res.on('end', () => {

          try {

            const parsedData = <T> JSON.parse(rawData);
            resolve(parsedData);
          } 
          catch (e) {
            reject(e.message);
          }
        });
      }).on("error", (e) => {
        reject(e);
      });
    });
  }
}
