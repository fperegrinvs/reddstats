## <reference path="lib/jasmine.js"/>
## <reference path="lib/jasmine-given.js"/>
## <reference path="../lib/angular.js"/>
## <reference path="../app.js"/>
Given -> @stateConfigs = {'oi':{},'ei':{},'epa':{}, 'carrinho': {target:'modal'}};
describe "CreateStateOptions: Given one valid state", -> 
    Given -> @states = ['oi']
    When -> @opts = createStateOptions(@states, @stateConfigs)
    Then "one option", ->  @opts.length == 1

describe "CreateStateOptions: Given one invalid state", -> 
    Given -> @states = ['oie']
    When -> @opts = createStateOptions(@states, @stateConfigs)
    Then "no options", ->  @opts.length == 0

describe "CreateStateOptions: Given two nested states", -> 
    Given -> @states = ['oi', 'oi.ei']
    When -> @opts = createStateOptions(@states, @stateConfigs)
    Then "two options", ->  @opts.length == 2
    Then "nested options", ->  @opts[1].name == "oi.ei"
    Then "option 2 targets layer 1", -> @opts[1].target == "layer1"

describe "CreateStateOptions: Given three nested states", -> 
    Given -> @states = ['oi', 'oi.ei', 'oi.ei.epa']
    When -> @opts = createStateOptions(@states, @stateConfigs)
    Then "option 3 targets layer 2", -> @opts[2].target == "layer2"

describe "CreateStateOptions: Given invalid nested state", -> 
    Given -> @states = ['oi', 'oi.ei', 'oi.ei.eba']
    When -> @opts = createStateOptions(@states, @stateConfigs)
    Then "creates only valid options", ->  @opts.length == 2


describe "CreateStateOptions: Given invalid root state", -> 
    Given -> @states = ['oi', 'oi.ei', 'ola.ei.epa']
    When -> @opts = createStateOptions(@states, @stateConfigs)
    Then "creates only valid options", ->  @opts.length == 2
    
describe "CreateStateOptions: Given a child modal state", ->
    Given -> @states = ['oi', 'oi.carrinho']
    When -> @opts = createStateOptions(@states, @stateConfigs)
    Then "child state target is still modal", -> @opts[1].target == "modal"