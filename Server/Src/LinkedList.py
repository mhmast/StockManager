class LinkedListRegistry:
    def __init__(self):
        self.first = None
        self.last = None
        self.objDict= {}

class LinkedListElement:

    def __init__(self, obj, registry):
        
        self.obj = obj
        objDict = registry.objDict
        if obj in objDict:
            objDict[obj].append(self)
        else:
            objDict[obj] = [self]
        
        self.registry = registry
        lastItem = registry.last
        if lastItem:
            lastItem.nextItem = self

        self.previousItem = lastItem
        if registry.first is None:
            registry.first = self
        registry.last = self
        self.nextItem = None

    def remove(self, item):

        objDict = self.registry.objDict

        for node in objDict[item]:
            if node.previousItem:
                node.previousItem.nextItem = node.nextItem
            if node.nextItem:
                node.nextItem.previousItem = node.previousItem
            if node == self.registry.last:
                self.registry.last = node.previousItem
            if node == self.registry.first:
                self.registry.first = node.nextItem
            node.nextItem = None
            node.previousItem = None
        
        objDict.pop(item)

    def next(self):
        return self.nextItem

    def append(self, item):
        return LinkedListElement(item,self.registry)
