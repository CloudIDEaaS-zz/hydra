export class ConnectPromise {
    
    value: string;
    successFunction: any;
    error: any;
    failFunction: any;

    resolve(value : string) {
        this.value = value;
        if (this.successFunction) {
            this.successFunction(value);
        }
    }

    reject(error) {
        this.error = error;
        if (this.failFunction) {
            this.failFunction(error);
        }
    }

    then(onfulfilled, onrejected = null) {
        if (this.value) {
            onfulfilled(this.value);
            return;
        }
        if (this.error) {
            onrejected(this.error);
            return;
        }
        this.successFunction = onfulfilled;
        this.failFunction = onrejected;
        return this;
    }

    catch(onrejected) {
        this.failFunction = onrejected;
        return this;
    }
}
