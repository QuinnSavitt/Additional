# Leetcode daily 5/2/2024, find largest integer k such that -k is also in array nums
def findMaxK(nums: List[int]) -> int:
        return max(r) if (r := [i for i in nums if i>0 and i*-1 in [j for j in nums if j < 0]]) else -1

# Leetcode Problem 1051. Wastes space by continuously resorting s, a two line solution would be more optimal (and it is what I have on Leetcode!) but for the purpose of codegolf it is one.
def heightChecker(self, heights: List[int]) -> int:
        return sum([1 for i in range(len(heights)) if heights[i] != sorted(heights)[i]])
