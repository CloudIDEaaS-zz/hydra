export class Utils {
    public static expandPath(path : string) : string {

        var replaced = path.replace(/%([^%]+)%/g, function(_,n) {
            return process.env[n];
        });

        return replaced;
    }

    public static sleep(time) : Promise<{}> {
        return new Promise((resolve) => setTimeout(resolve, time));
    }

    public static pause(time : number) {
        let start = Date.now();

        while (true) {
            
            let now = Date.now();

            if (now - start >= time) {
                break;
            }
        }
    }
}
