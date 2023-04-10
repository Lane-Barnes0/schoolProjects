import java.util.LinkedList;

public class TaskLRU implements Runnable{
    int[] sequence;
    int maxMemoryFrames;
    int maxPageReference;
    int[] pageFaults;
    int numberOfFaults = 0;
    LinkedList currentPages = new LinkedList();

    public TaskLRU(int[] sequence , int maxMemoryFrames, int maxPageReference, int[] pageFaults) {
        this.sequence = sequence;
        this.maxMemoryFrames = maxMemoryFrames;
        this.maxPageReference = maxPageReference;
        this.pageFaults = pageFaults;
    }

    @Override
    public void run() {
        for (int i = 0; i < sequence.length; i++) {
            if (!currentPages.contains(sequence[i])) {
                currentPages.add(sequence[i]);
                numberOfFaults++;
                if (currentPages.size() > maxMemoryFrames) {
                    currentPages.removeFirst();
                }
            } else {
                currentPages.add(sequence[i]);
                currentPages.removeFirstOccurrence(sequence[i]);
            }
        }
        pageFaults[maxMemoryFrames - 1] = numberOfFaults;
    }
}
