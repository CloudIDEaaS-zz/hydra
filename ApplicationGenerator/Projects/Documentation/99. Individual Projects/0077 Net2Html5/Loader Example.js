
// functional instantiation of ExternalObject

var externalObject = ExternalObjectWrapper();       // see A. below, the true object does not get instantiated here

externalObject.method1();   // first call to a method or property

// A. the ExternalObjectWrapper is actually part fo the same calling Assembly.  It gets created based on AssemblyRefs, TypeRefs, and MemberRefs

var ExternalObjectWrapper = function ()
{
    this.thisAssembly = null;           // preset to calling Assembly, see B. below
    this.assemblyRef = 16;              // this is an AssemblyRef Token (int)
    this.typeRef = 28;                  // this is a TypeRef Token (int)
    this.externalAssembly = null;
    this.externalType = null;
    this.externalObjectInstance = null;
    this.constructorTypeArgs = null;
    this.constructorArgs = null;
    this.memberMap = MemberMap();       // dictionary of members

    funcMethods.constructor1 = function ()
    {
        var memberRef = 7;                                  // this is a MemberRef Token for constructor (int)
        var member = thisAssembly.GetMember(memberRef);     // not stored in memberMap since can only be called once

        // validate arguments if any
        
        member.ValidateArgs(...);

        // set up args based on what's passed in

        constructorTypeArgs = new object[];
        constructorArgs = new object[];
    }

    funcMethods.method1 = function(arg1, arg2)
    {
        var memberRef = 8;
        var member = null;
        var memberInstance;

        // check if true object is loaded

        if (externalAssembly == null)
        {
            var constructorInfo;

            // load assembly

            externalAssembly = thisAssembly.LoadExternalAssembly(assemblyRef);
            externalType = externalAssembly.GetType(typeRef);

            constructorInfo = externalType.GetConstructor(constructorTypeArgs);

            externalObjectInstance = constructorInfo.Invoke(constructorArgs);
        }

        if (memberMap.Contains(memberRef))
        {
            memberInstance = memberMap[memberRef];      // this member mapping concept will allow for polymorphism and method overriding

            member = memberInstance.member;
        }
        else
        {
            member = thisAssembly.GetMember(memberRef);
            memberInstance = MemberInstance(member);

            memberInstance.member = member;
        }

        // validate arguments
        
        member.ValidateArg(arg1, arg2);

        // call actual member
        
        memberInstance.Invoke(arg1, arg2);
    }
}

// B. Assembly - for our example, represents calling Assembly

var Assembly = function ()
{
    this.peImage = null; // preset to calling PE Image once loaded

    funcMethods.LoadExternalAssembly = function(assemblyRef)
    {
        var assemblyRef = GetAssemblyRef(assemblyRef);  // gets assemblyRef from PE Image
        var assemblyName = assemblyRef.GetName();       // gets the strong name from assemblyRef (name, culture, public key, etc)

        // this calls into Fusion, loading assembly from server

        return Assembly.LoadAssembly(assemblyName);  // see C. below
    }
}

// C. Fusion Loader

/*

fusion api is only javascript file download as-is to client

Client:

get server public key
create client private key/public key pair (public key = client key)
encrypt client key with server public key
sign key
send client key and signature to server  (see D. below)

Server:

validate signature
decrypt client key
obfuscate code units
encrypt code units with client key
add code units to pe file (assembly)
send pe file

Client:

download pe file 
decrypt code units with client private key

See the following:

https://github.com/diafygi/webcrypto-examples#rsa-pss
https://www.w3.org/TR/WebCryptoAPI/

*/

// D. Server Call

/*

This server call uses dynamically created script from the server to obfuscate the call (series of calls) to the backend.
It creates the client key but then passes it around (via unpredictable Passage ways), taking it through a series of scramblers before calling server 
Scramblers include xor operations, custom scramblers, timestamps checks (to assure not being debugged)

Checks and sends:

call stack (to assure being called from Fusion API)
anti-forgery cookie
custom cookies
session id

Passage ways:

hidden fields
data attributes
cookies
binary files - hidden in images?
obfuscation code (auto-generated)
threads
sockets
web storage

Call series includes:

Ajax
Web Sockets
Server-Sent Events

*/