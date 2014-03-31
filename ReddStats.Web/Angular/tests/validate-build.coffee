## <reference path="lib/jasmine.js"/>
## <reference path="lib/jasmine-given.js"/>

describe "build", -> 
    When -> @oi = true
    Then "test passes", -> @oi == true
