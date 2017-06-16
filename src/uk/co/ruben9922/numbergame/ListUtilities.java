package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

import java.util.List;
import java.util.ListIterator;
import java.util.Scanner;

public class ListUtilities {
    public static <E> void printList(List<E> list, boolean showIndices) {
        ListIterator<E> listIterator = list.listIterator();
        while (listIterator.hasNext()) {
            E element = listIterator.next();
            int index = listIterator.nextIndex();
            if (showIndices) {
                System.out.format("[%d] ", index);
            }
            System.out.format("%s\n", element.toString());
        }
    }

    public static <E> int chooseItem(Scanner scanner, @NotNull List<E> list, @Nullable String prompt) {
        // Assign default value to parameter if null
        if (prompt == null) {
            prompt = String.format("Item number [%d..%d]: ", 0, list.size() - 1);
        }

        printList(list, true);
        return InputUtilities.inputInt(scanner, prompt, 0, list.size());
    }

    public static <E> int chooseItem(Scanner scanner, @NotNull List<E> list) {
        return chooseItem(scanner, list, null);
    }
}
