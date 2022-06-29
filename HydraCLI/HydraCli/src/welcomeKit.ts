const center = require('center-align');
const chalk = require('chalk');
const hydraGreen = chalk.rgb(10, 126, 121);
const hydraBrown = chalk.rgb(121, 85, 1);
const gray = chalk.rgb(146, 146, 146);
const blue = chalk.rgb(62, 161, 255);

export class WelcomeKit {
    public static start() {

        let lines = [];

        lines.push(hydraGreen(center("_".repeat(75), 75)));
        lines.push("");
        lines.push(hydraGreen.bold(center("Thank you for installing Hydra!", 75)));
        lines.push(hydraBrown(center("Copyright © 2020 CloudIDEaaS Inc", 75)));

        lines.push("");
        lines.push(gray(center("For help getting started, visit:", 75)));
        lines.push(blue(center("https://www.cloudideaas.com/hydra/index.htm", 75)));

        lines.push(hydraGreen(center("_".repeat(75), 75)));

        lines.forEach(l => {
            console.log(l);
        });
    }
}

