#include <vector>
#include <queue>
using namespace std;

// TreeNode class should go in the "ufl_cap4053::fundamentals" namespace!
namespace ufl_cap4053 { namespace fundamentals {
	template <typename T> class TreeNode {
	private:
		T data;
		vector<TreeNode<T>*> children;

	public:
		TreeNode<T>();
		TreeNode<T>(T element);
		
		const T& getData() const;
		size_t getChildCount() const;

		TreeNode<T>* getChild(size_t index);
		TreeNode<T>* getChild(size_t index) const;

		void addChild(TreeNode<T>* child);
		TreeNode<T>* removeChild(size_t index);

		void breadthFirstTraverse(void (*dataFunction)(const T)) const;
		void preOrderTraverse(void (*dataFunction)(const T)) const;
		void postOrderTraverse(void (*dataFunction)(const T)) const;
	};

	template <typename T> TreeNode<T>::TreeNode() {
		data = T();
		children = vector<TreeNode<T>*>();
	}

	template <typename T> TreeNode<T>::TreeNode(T element) {
		data = element;
		children = vector<TreeNode<T>*>();
	}

	template <typename T> const T& TreeNode<T>::getData() const {
		return data;
	}

	template <typename T> size_t TreeNode<T>::getChildCount() const {
		return children.size();
	}

	template <typename T> TreeNode<T>* TreeNode<T>::getChild(size_t index) {
		return children[index];
	}

	template <typename T> TreeNode<T>* TreeNode<T>::getChild(size_t index) const {
		return children[index];
	}

	template <typename T> void TreeNode<T>::addChild(TreeNode<T>* child) {
		return children.push_back(child);
	}

	template <typename T> TreeNode<T>* TreeNode<T>::removeChild(size_t index) {
		TreeNode<T>* ret = children[index];
		children.erase(children.begin() + index);
		return ret;
	}

	template <typename T> void TreeNode<T>::breadthFirstTraverse(void (*dataFunction)(const T)) const {
		queue<TreeNode<T>*> q = queue<TreeNode<T>*>();
		q.push((TreeNode<T>*)this);

		while (!q.empty()) {
			TreeNode<T>* curr = q.front();
			q.pop();
			dataFunction(curr->data);
			for (TreeNode<T>* child : curr->children) {
				q.push(child);
			}
		}

		return;
	}

	template <typename T> void TreeNode<T>::preOrderTraverse(void (*dataFunction)(const T)) const {
		dataFunction(data);
		for (TreeNode<T>* child : children) {
			child->preOrderTraverse(dataFunction);
		}
		return;
	}

	template <typename T> void TreeNode<T>::postOrderTraverse(void (*dataFunction)(const T)) const {
		for (TreeNode<T>* child : children) {
			child->postOrderTraverse(dataFunction);
		}
		dataFunction(data);
		return;
	}



}}  // namespace ufl_cap4053::fundamentals


