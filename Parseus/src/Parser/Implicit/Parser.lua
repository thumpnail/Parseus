Context = {
    _tokens = {},
    _currentToken = nil,
    _currentIndex = 1
}
function Context.new()
    local self = setmetatable({}, { __index = Context })
    self._tokens = {}
    self._currentToken = nil
    self._currentIndex = 1
    return self
end
function Context:Consume()
    if self._currentIndex <= #self._tokens then
        self._currentToken = self._tokens[self._currentIndex]
        self._currentIndex = self._currentIndex + 1
    else
        self._currentToken = nil
    end
    return self._currentToken
end
function Context:Peek()
    return self._tokens[self._currentIndex]
end

Parser = {
    context = Context.new()
}
-- create new metatable for Parser
function Parser.new()
    local self = setmetatable({}, { __index = Parser })
    self._tokens = {}
    self._currentToken = nil
    self._currentIndex = 1
    return self
end
function Token(context, token, out)
    context:Consume()
end