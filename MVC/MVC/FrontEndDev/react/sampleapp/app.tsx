import * as ReactDOM from 'react-dom';
import * as React from 'react';
import {HelloWorld} from './components/HelloWorld'


// declare my helper in the window interface
declare global {
    interface Window {
        ReactMethods: ReactMethods
    }
}
export class ReactMethods {
    static SampleApp(containerID : string): void {
        ReactDOM.render(
            <HelloWorld CustomMessage="Testing etc" />
            , document.getElementById(containerID));
    }
}

// Can access your methods with window.ReactMethods.SampleApp("MyContainer")
window.ReactMethods = ReactMethods;
