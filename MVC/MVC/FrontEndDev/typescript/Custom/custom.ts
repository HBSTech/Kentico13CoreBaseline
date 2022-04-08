/* Content here */
export function testCustom(): Test {
    var testing: Test = { message: "Hello World" };
    return testing;
}

interface Test {
    message: string;
}

document.addEventListener("ready", function () {
    var test = testCustom();
});