/* Content here */
function test() : Test {
    var testing : Test = { message: "Hello World2" };
    alert(testing.message);
    alert("Hello World!6");
    return testing;
}

export interface Test {
    message: string;
}