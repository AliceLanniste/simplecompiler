function getName(): string
{
    print("What's your name?")
    let name = input()
    return name
}

print("Hello ")

let name = getName()
print("Hello, " + name)