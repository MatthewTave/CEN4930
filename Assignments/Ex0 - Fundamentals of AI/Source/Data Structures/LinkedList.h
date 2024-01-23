

// LinkedList class should go in the "ufl_cap4053::fundamentals" namespace!
namespace ufl_cap4053 { namespace fundamentals {
	template <typename T> class LinkedList {
	private:
		struct Node {
			T data = T();
			Node *prev = nullptr, *next = nullptr;
		};

		Node *beginNode, *endNode;

	public:
		class Iterator {
		private:
			Node* targetNode;
		public:
			Iterator(Node* targetNode);
			T operator*() const;
			Iterator& operator++();
			bool operator==(Iterator const& rhs);
			bool operator!=(Iterator const& rhs);
		};

		LinkedList<T>();
		~LinkedList<T>();

		Iterator begin() const;
		Iterator end() const;
		
		bool isEmpty() const;
		
		T getFront() const;
		T getBack() const;
		
		void enqueue(T element);
		void dequeue();
		
		void pop();
		void clear();
		
		bool contains(T element) const;
		
		void remove(T element);
	};

	// Iterator Functions

	template <typename T> LinkedList<T>::Iterator::Iterator(Node* targetNode) {
		this->targetNode = targetNode;
	}

	template <typename T> T LinkedList<T>::Iterator::operator*() const {
		return targetNode->data;
	}

	template <typename T> typename LinkedList<T>::Iterator& LinkedList<T>::Iterator::operator++() {
		targetNode = targetNode->next;
		return *this;
	}

	template <typename T> bool LinkedList<T>::Iterator::operator==(Iterator const& rhs) {
		return targetNode == rhs.targetNode;
	}

	template <typename T> bool LinkedList<T>::Iterator::operator!=(Iterator const& rhs) {
		return targetNode != rhs.targetNode;
	}

	// Linked List Functions

	template <typename T> LinkedList<T>::LinkedList() {
		beginNode = new Node();
		this->endNode = beginNode;
	}

	template <typename T> LinkedList<T>::~LinkedList() {
		clear();
		delete endNode;
	}

	template <typename T> LinkedList<T>::Iterator LinkedList<T>::begin() const {
		return Iterator(beginNode);
	}

	template <typename T> LinkedList<T>::Iterator LinkedList<T>::end() const {
		return Iterator(endNode);
	}

	template <typename T> bool LinkedList<T>::isEmpty() const {
		return beginNode == endNode;
	}

	template <typename T> T LinkedList<T>::getFront() const {
		return isEmpty() ? T() : beginNode->data;
	}

	template <typename T> T LinkedList<T>::getBack() const {
		return isEmpty() ? T() : endNode->prev->data;
	}

	template <typename T> void LinkedList<T>::enqueue(T element) {
		Node* newNode = new Node();

		newNode->prev = endNode->prev;
		newNode->next = endNode;

		newNode->data = element;
		
		if (isEmpty()) {
			beginNode = newNode;
		}
		else {
			endNode->prev->next = newNode;
		}

		endNode->prev = newNode;
	}

	template <typename T> void LinkedList<T>::dequeue() {
		if (isEmpty()) return;
		
		beginNode = beginNode->next;
		delete beginNode->prev;
		beginNode->prev = nullptr;
	}

	template <typename T> void LinkedList<T>::pop() {
		if (isEmpty()) return;

		if (endNode->prev->prev != nullptr) {
			endNode->prev->prev->next = endNode;
		}
		else {
			beginNode = endNode;
		}

		Node* delTarget = endNode->prev;
		endNode->prev = endNode->prev->prev;

		delete delTarget;
	}

	template <typename T> void LinkedList<T>::clear() {
		while (!isEmpty()) pop();
	}

	template <typename T> bool LinkedList<T>::contains(T element) const {
		for (Node* ptr = beginNode; ptr != endNode; ptr = ptr->next) if (ptr->data == element) return true;
		return false;
	}

	template <typename T> void LinkedList<T>::remove(T element) {
		if (isEmpty()) return;
		
		Node* ptr = beginNode;
		for (; ptr->data != element && ptr != endNode; ptr = ptr->next);
		
		if (ptr == endNode) return;

		if (ptr == beginNode) {
			dequeue();
		}
		else if (ptr == endNode->prev) {
			pop();
		}
		else {
			ptr->prev->next = ptr->next;
			ptr->next->prev = ptr->prev;
			delete ptr;
		}
	}

} }  // namespace ufl_cap4053::fundamentals
