using Microsoft.VisualStudio.TestTools.UnitTesting;

// Tests cover insertion order, priority selection, FIFO ties, removal and empty queues.

[TestClass]
public class PriorityQueueTests
{
    [TestMethod]
    // Scenario: Enqueue items in mixed priority order, with the highest priority last.
    // Expected Result: Items remain in insertion order until dequeued; highest priority leaves first.
    // Defect(s) Found: The search skips the last item and Dequeue does not remove the selected item.
    // Result: Initially returned Middle instead of High; passed after the fixes.
    public void TestPriorityQueue_1()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Low", 1);
        priorityQueue.Enqueue("Middle", 4);
        priorityQueue.Enqueue("High", 9);
        Assert.AreEqual("[Low (Pri:1), Middle (Pri:4), High (Pri:9)]", priorityQueue.ToString());
        Assert.AreEqual("High", priorityQueue.Dequeue());
        Assert.AreEqual("Middle", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
        Assert.AreEqual("[]", priorityQueue.ToString());
    }

    [TestMethod]
    // Scenario: Enqueue two equally high priority items followed by a lower priority item.
    // Expected Result: The first high priority item leaves before the second.
    // Defect(s) Found: Using >= selects a later item when priorities tie.
    // Result: Initially returned Second instead of First; passed after the fixes.
    public void TestPriorityQueue_2()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("First", 5);
        priorityQueue.Enqueue("Second", 5);
        priorityQueue.Enqueue("Low", 1);
        Assert.AreEqual("First", priorityQueue.Dequeue());
        Assert.AreEqual("Second", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Dequeue from a newly created queue.
    // Expected Result: InvalidOperationException with the required message.
    // Defect(s) Found: None; the original empty queue check passes.
    // Result: Passed before and after the fixes.
    public void TestPriorityQueue_Empty()
    {
        var priorityQueue = new PriorityQueue();
        var exception = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", exception.Message);
    }

    [TestMethod]
    // Scenario: Remove the only item, try removing again, then reuse the queue.
    // Expected Result: The queue becomes empty, throws the required exception and accepts a new item.
    // Defect(s) Found: Dequeue returns the value without removing it.
    // Result: Initially did not throw after removing the only item; passed after the fixes.
    public void TestPriorityQueue_RemovalAndReuse()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Only", 0);
        Assert.AreEqual("Only", priorityQueue.Dequeue());
        var exception = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", exception.Message);
        priorityQueue.Enqueue("New", -2);
        Assert.AreEqual("New", priorityQueue.Dequeue());
        Assert.AreEqual("[]", priorityQueue.ToString());
    }

    [TestMethod]
    // Scenario: Interleave insertion and removal with negative priorities and a tie at the back.
    // Expected Result: Larger priorities leave first and tied items retain arrival order.
    // Defect(s) Found: Returned items remain in the queue and the last item is skipped.
    // Result: Initially returned Second instead of Urgent; passed after the fixes.
    public void TestPriorityQueue_InterleavedNegativePriorities()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("First", -1);
        priorityQueue.Enqueue("Low", -10);
        priorityQueue.Enqueue("Second", -1);
        Assert.AreEqual("First", priorityQueue.Dequeue());
        priorityQueue.Enqueue("Urgent", 0);
        Assert.AreEqual("Urgent", priorityQueue.Dequeue());
        Assert.AreEqual("Second", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
        Assert.AreEqual("[]", priorityQueue.ToString());
    }
}
