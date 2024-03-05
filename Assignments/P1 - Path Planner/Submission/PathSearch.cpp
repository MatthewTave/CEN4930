#include "PathSearch.h"

#include <chrono>
#include <iostream>
#include <cmath>

// Colors for drawing
#define RED 0xFFFF0000
#define YELLOW 0xFFFFFF33
#define GREEN 0xFF33FF33

// Comparison Functions for Priority Queue

bool bfs(const pathNode& lhs, const pathNode& rhs) {
	return true;
}

bool greedyBfs(const pathNode& lhs, const pathNode& rhs) {
	return hypot(lhs.coords.first - lhs.t.first, lhs.coords.second - lhs.t.second) > hypot(rhs.coords.first - rhs.t.first, rhs.coords.second - rhs.t.second);
}

bool ucs(const pathNode& lhs, const pathNode& rhs) {
	return lhs.totalCost > rhs.totalCost;
}

bool aSTAR(const pathNode& lhs, const pathNode& rhs) {
	return hypot(lhs.coords.first - lhs.t.first, lhs.coords.second - lhs.t.second) + lhs.totalCost > hypot(rhs.coords.first - rhs.t.first, rhs.coords.second - rhs.t.second) + rhs.totalCost;
}

// Adjeceny Utility Functions

bool areAdj(const pair<int, int>& lhs, const pair<int, int>& rhs) {
	if (lhs.first == rhs.first) {
		return lhs.second + 1 == rhs.second || lhs.second - 1 ==  rhs.second;
	}
	else if (lhs.second == rhs.second) {
		return lhs.first + 1 == rhs.first || lhs.first - 1 == rhs.first;
	}
	else if (lhs.first % 2 == 0) {
		return lhs.second - 1 == rhs.second && (lhs.first + 1 == rhs.first || lhs.first - 1 == rhs.first);
	}
	else {
		return lhs.second + 1 == rhs.second && (lhs.first + 1 == rhs.first || lhs.first - 1 == rhs.first);
	}
}

void getAdj(const pair<int, int> curr, ufl_cap4053::TileMap* map, vector<vector<pair<int, int>>>& prev, vector<pair<int, int>>& ret) {
	vector<pair<int, int>> adj = {
		{0, 1},
		{0, -1},
		{1, 0},
		{-1, 0},
		{1, 1},
		{1, -1},
		{-1, 1},
		{-1, -1}
	};

	ret.clear();
	for (pair<int, int> a : adj) {
		pair<int, int> t = {curr.first + a.first, curr.second + a.second};

		if (t.first >= 0 && 
			t.second >= 0 && 
			t.first < map->getRowCount() && 
			t.second < map->getColumnCount() && 
			map->getTile(t.first, t.second)->getWeight() > 0 &&
			areAdj(curr, t)) 
		{
			ret.push_back(t);
		}
	}
}

namespace ufl_cap4053
{
	namespace searches
	{
		PathSearch::PathSearch() : q(bfs) {
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
			s = { startRow, startCol};
			t = { goalRow, goalCol};

			q = PriorityQueue(aSTAR);
			q.push(pathNode(s, 0, 0, t));

			cost = vector<vector<int>>(map->getRowCount(), vector<int>(map->getColumnCount(), -1));
			prev = vector<vector<pair<int, int>>>(map->getRowCount(), vector<pair<int, int>>(map->getColumnCount(), { -1, -1 }));
			
			prev[s.first][s.second] = s;
			cost[s.first][s.second] = 0;
		}

		void PathSearch::update(long timeslice) {
			auto start = chrono::high_resolution_clock::now();
			while (!isDone() && !q.empty()) {
				curr = q.front().coords;
				q.pop();

				map->getTile(curr.first, curr.second)->setMarker(GREEN);
				
				vector<pair<int, int>> adj;
				getAdj(curr, map, prev, adj);
				for (pair<int, int> a : adj) {
					bool newNode = (cost[a.first][a.second] == -1);

					if (cost[a.first][a.second] == -1 || cost[a.first][a.second] > cost[curr.first][curr.second] + map->getTile(a.first, a.second)->getWeight()) {
						prev[a.first][a.second] = curr;
						cost[a.first][a.second] = cost[curr.first][curr.second] + map->getTile(a.first, a.second)->getWeight();
					}

					if (newNode) {
						q.push(pathNode(a, map->getTile(a.first, a.second)->getWeight(), cost[a.first][a.second], t));
						map->getTile(a.first, a.second)->setMarker(YELLOW);
					}
				}

				if (chrono::duration_cast<std::chrono::milliseconds>(chrono::high_resolution_clock::now() - start).count() >= timeslice) {
					return;
				}
			}
		}

		void PathSearch::shutdown() {}

		void PathSearch::unload() {
			map = nullptr;
		}

		bool PathSearch::isDone() const {
			return !(prev.empty() || prev[t.first][t.second] == pair<int, int>{-1, -1});
		}

		vector<Tile const*> const PathSearch::getSolution() const {
			vector<Tile const*> ret;

			if (isDone()) {
				pair<int, int> curr = t;
				ret.push_back(map->getTile(curr.first, curr.second));

				while (curr != s) {
					map->getTile(curr.first, curr.second)->addLineTo(map->getTile(prev[curr.first][curr.second].first, prev[curr.first][curr.second].second), RED);

					curr = prev[curr.first][curr.second];
					ret.push_back(map->getTile(curr.first, curr.second));
				}

				cout << "Total Path Cost: " << cost[t.first][t.second] << "\n";
			}

			return ret;
		}
	}
}  // close namespace ufl_cap4053::searches
