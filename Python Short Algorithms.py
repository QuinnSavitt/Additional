# Inspired by LeetCode's Climbing Stairs Challenge, how many combinations of stairs can you take in a staircase of length 
class StairClimbing:
  stored = {0:0, 1:1, 2:2}
  def climbStairs(self, n):
          return self.stairHelper(n)
  def stairHelper(self, n):
      if n in self.stored:
          return self.stored[n]
      add = self.stairHelper(n-1) + self.stairHelper(n-2)
      self.stored[n] = add
      return add
