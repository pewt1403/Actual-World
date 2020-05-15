

int main()
{
    int a[100][100];
    int b[100][100];
    int c[100][100];

    for (int j = 0; j < 100; j++)
    {
        for (int i = 0; i < 100; i++)
        {
            c[i][j] = a[i][j] + b[i][j];
        }
    }

    struct Data
    {
        /* data */
        int a;
        int b;
        int c;
    };

    Data data[100][100];

    for (int i = 0; i < 100; i++)
    {
        for (int j = 0; j < 100; j++)
        {
            c[i][j] = a[i][j] + b[i][j];
        }
    }
}
