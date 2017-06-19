package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

import java.util.List;
import java.util.ListIterator;
import java.util.Scanner;

public class ListUtilities {
    public static <E> void printList(@NotNull List<E> list, boolean showIndices) {
        ListIterator<E> listIterator = list.listIterator();
        while (listIterator.hasNext()) {
            int index = listIterator.nextIndex();
            E element = listIterator.next();
            if (showIndices) {
                System.out.format("[%d] ", index);
            }
            System.out.format("%s\n", element.toString());
        }
    }

    @Nullable
    public static <E> Integer chooseItem(Scanner scanner, @NotNull List<E> list, @Nullable String prompt) {
        if (list.size() == 0) {
            return null;
        }

        // If exactly one item in list then choose that item (don't bother asking user to choose item)
        if (list.size() == 1) {
            System.out.println("Only 1 item in list so picking that item"); // TODO: Possibly change messages
            return 0;
        }

        // Assign default value to parameter if null
        if (prompt == null) {
            prompt = String.format("Item number [%d..%d]: ", 0, list.size() - 1);
        }

        // Simply print all items in list
        printList(list, true);

        return InputUtilities.inputInt(scanner, prompt, 0, list.size());
    }

    @Nullable
    public static <E> Integer chooseItem(Scanner scanner, @NotNull List<E> list) {
        return chooseItem(scanner, list, null);
    }
}
