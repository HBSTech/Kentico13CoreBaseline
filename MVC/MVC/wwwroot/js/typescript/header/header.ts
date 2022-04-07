/* Content here */
function test() : Test {
    var testing : Test = { message: "Hello World2" };
    alert(testing.message);
    var testing2: Test = { message: "Hello World3" };
    var testing4: Test = { message: "Hello World5" };
    return testing;
}

export interface Test {
    message: string;
}