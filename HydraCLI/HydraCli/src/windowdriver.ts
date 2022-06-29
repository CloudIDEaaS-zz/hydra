const assert = require('assert');
const path = require('path');
const Application = require('spectron').Application;
const electronPath = require('electron');

const driverApp = new Application({
  path: electronPath,
  args: [path.join(__dirname, '..')]
});

export class Driver {
  public static async start() {

    driverApp.start();

    const count = await driverApp.client.getWindowCount();
    
  }
}

