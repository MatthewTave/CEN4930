#include "PathSearch.h"

#include <chrono>

// Comparison Functions for Priority Queue

bool queue(const pair<int, int>& lhs, const pair<int, int>& rhs) {
	return true;
}

namespace ufl_cap4053
{
	namespace searches
	{
		PathSearch::PathSearch() : q(queue) {
			map = nullptr;

			curr = { 0, 0 };
			s = { 0, 0 };
			t = { 0, 0 };
		}

		PathSearch::~PathSearch() {}

		void PathSearch::load(TileMap* _tileMap) {
			map = _tileMap;
		}

		void PathSearch::initialize(int startRow, int startCol, int goalRow, int goalCol) {
			s = { startRow, startCol };
			t = { goalRow, goalCol };

			q = PriorityQueue(queue);
			
			cost = vector<vector<int>>(map->getRowCount(), vector<int>(map->getColumnCount(), -1));
			prev = vector<vector<pair<int, int>>>(map->getRowCount(), vector<pair<int, int>>(map->getColumnCount(), {-1, -1}));

			q.push(s);
			
			prev[s.first][s.second] = s;
			cost[s.first][s.second] = 0;
		}

		void PathSearch::update(long timeslice) {
			auto start = chrono::high_resolution_clock::now();
			while (!isDone()) {

				// Search

				if (chrono::duration_cast<std::chrono::milliseconds>(chrono::high_resolution_clock::now() - start).count() >= timeslice) {
					return;
				}
			}
		}

		void PathSearch::shutdown() {
		}

		void PathSearch::unload() {
		}

		bool PathSearch::isDone() const {
			return prev[t.first][t.second] != pair<int, int>{-1, -1};
		}

		vector<Tile const*> const PathSearch::getSolution() const {
			return vector<Tile const*>();
		}
	}
}  // close namespace ufl_cap4053::searches
