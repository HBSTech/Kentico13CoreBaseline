/* Content here */
export function testHeader(): Test {
    var testing: Test = { message: "Hello World2", test2: 12 };
    return testing;
}

interface Test {
    message: string;
    test2: number;
}

document.addEventListener("ready", function () {
    var test = testHeader();
});