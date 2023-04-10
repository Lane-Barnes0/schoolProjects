import java.util.ArrayList;
import java.util.Random;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

public class Main {
    public static void main(String[] args) {
        long timeStart = System.currentTimeMillis();
        int anomalies = 0;

        int [][] pageFs = new int[1000][100];
        int [][] pageFsMru = new int[1000][100];
        int [][] pageFsLru = new int[1000][100];

        int fifoMinPF = 0;
        int mruMinPF = 0;
        int lruMinPF = 0;

        int simulation = 0;
        for (int j = 0; j < 1000; j++) {
            ExecutorService threadPool = Executors.newFixedThreadPool(Runtime.getRuntime().availableProcessors());

            int[] pageFaults = new int[100];
            int[] lruPageFaults = new int[100];
            int[] mruPageFaults = new int[100];

            //Generate a randomized page reference sequence of length 1000 drawn from the interval [1,250]
            int[] p = new int[1000];
            Random rand = new Random();
            for (int i = 0; i < 1000; i++) {
                p[i] = rand.nextInt(251);
            }
            //Frames
            int f = 1;
            //For each count of main memory frames from [1, 100]
            while(f < 101) {
                Runnable fifo = new TaskFIFO(p, f, 250, pageFaults);
                Runnable lru = new TaskLRU(p,f,250,lruPageFaults);
                Runnable mru = new TaskMRU(p,f,250,mruPageFaults);
                threadPool.execute(fifo);
                threadPool.execute(lru);
                threadPool.execute(mru);
                f++;
            }
            threadPool.shutdown();
            try {
                threadPool.awaitTermination(Long.MAX_VALUE, TimeUnit.DAYS);
            }
            catch (Exception ex) {
                System.out.println("Error in waiting for shutdown");
            }

            for (int i = 0; i < pageFaults.length - 1; i++) {
                pageFs[j][i] = pageFaults[i];
            }
            for(int i = 0; i < mruPageFaults.length -1; i++) {
                pageFsMru[j][i] = mruPageFaults[i];
            }
            for(int i = 0; i < lruPageFaults.length -1; i++) {
                pageFsLru[j][i] = lruPageFaults[i];
            }

        }

        long timeEnd = System.currentTimeMillis();
        System.out.printf("Simulation took: %.3f seconds\n\n", (timeEnd - timeStart) / 1000.0);

        for(int j = 0; j < pageFs.length - 1 ; j ++) {
            for (int i = 0; i < pageFs[i].length - 1; i++) {
                if (pageFs[j][i] < pageFsMru[j][i] && pageFs[j][i] < pageFsLru[j][i] ) {
                    fifoMinPF++;
                } else if(pageFsMru[j][i] < pageFs[j][i] && pageFsMru[j][i] < pageFsLru[j][i]) {
                    mruMinPF++;
                } else if(pageFsLru[j][i] < pageFsMru[j][i] && pageFsLru[j][i] < pageFs[j][i]) {
                    lruMinPF++;
                } else {
                    fifoMinPF++;
                    mruMinPF++;
                    lruMinPF++;
                }
            }
        }

        System.out.println("FIFO min PF: " + fifoMinPF);
        System.out.println("LRU min PF: " + lruMinPF);
        System.out.println("MRU min PF: " + mruMinPF);
        System.out.println();

        System.out.println("Belady's Anomaly Report for FIFO");
        for(int j = 0 ; j < pageFs.length - 1; j++) {
            simulation++;
            for (int i = 0; i < pageFs[i].length - 1; i++) {
                if (pageFs[j][i] < pageFs[j][i + 1]) {
                    anomalies++;
                    System.out.println("\tAnomaly detected in simulation #" + simulation + " - " + (pageFs[j][i]) +
                            " PF's @ " + i + " frames vs. " + (pageFs[j][i + 1]) + " PF's @ " + (i + 1) + " frames");
                }
            }
        }

        System.out.println("Anomaly detected " + anomalies + " times in 1000 simulations\n");

        System.out.println("Belady's Anomaly Report for LRU");
        simulation = 0;
        anomalies = 0;
        for(int j = 0 ; j < pageFsLru.length - 1; j++) {
            simulation++;
            for (int i = 0; i < pageFsLru[i].length - 1; i++) {
                if (pageFsLru[j][i] < pageFsLru[j][i + 1]) {
                    anomalies++;
                    System.out.println("\tAnomaly detected in simulation #" + simulation + " - " + (pageFsLru[j][i]) +
                            " PF's @ " + i + " frames vs. " + (pageFsLru[j][i + 1]) + " PF's @ " + (i + 1) + " frames");
                }
            }
        }

        System.out.println("\tAnomaly detected " + anomalies + " times in 1000 simulations\n");

        System.out.println("Belady's Anomaly Report for MRU");
        simulation = 0;
        anomalies = 0;
        for(int j = 0 ; j < pageFsMru.length - 1; j++) {
            simulation++;
            for (int i = 0; i < pageFsMru[i].length - 1; i++) {
                if (pageFsMru[j][i] < pageFsMru[j][i + 1]) {
                    anomalies++;
                    System.out.println("\tAnomaly detected in simulation #" + simulation + " - " + (pageFsMru[j][i]) +
                            " PF's @ " + i + " frames vs. " + (pageFsMru[j][i + 1]) + " PF's @ " + (i + 1) + " frames");
                }
            }
        }
        System.out.println("\tAnomaly detected " + anomalies + " times in 1000 simulations\n");
    }

}
