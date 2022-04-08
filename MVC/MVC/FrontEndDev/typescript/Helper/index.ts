import { SiteMethods } from './Helpers/SiteMethods'

// declare my helper in the window interface
declare global {
    interface Window {
        SiteMethods: SiteMethods
    }
}

// Can access your methods with window.SiteMethods.theMethodName()
window.SiteMethods = SiteMethods;
