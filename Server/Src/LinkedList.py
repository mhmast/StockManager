class LinkedListElement:

    def __init__(self, lst, registry={}, previous=None):
        obj = lst[0]
        self.obj = obj
        if obj in registry:
            registry[obj].append(self)
        else:
            registry[obj] = [self]

        self.registry = registry
        self.__previousItem = previous if previous is not None else self
        self.__nextItem = None
        self.__initialized = False
        self.__rest = None
        if len(lst) > 1:
            self.__rest = lst[1:]

    def initialize(self):
        if not self.__initialized and self.__rest is not None:
            self.__nextItem = LinkedListElement(self.__rest, self)
            self.__initialized = True

    def remove(self, item):
        self.initialize()
        current = self
        result = self if self.obj == item else self.__nextItem
        while current is not None:
            current.initialize()
            if current.obj == item:
                current.__previousItem.__nextItem = current.__nextItem
            current = current.next()
        return result

    def next(self):
        self.initialize()
        return self.__nextItem

    def append(self, item):
        self.initialize()
        current = self
        while current.__nextItem is not None:
            current = current.next()
            current.initialize()
        current.__nextItem = LinkedListElement([item], current)
