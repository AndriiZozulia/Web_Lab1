using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace lab1
{
    class Node
    {
        public int index;
        public ArrayList edges = new ArrayList();
        public bool exist = true;
        public int pow = 0;
        public Node(int i)
        {
            this.index = i;
        }
    }

    internal class Program
    {
        public Stack deleted_nodes = new Stack();
        private ArrayList briges = new ArrayList();
        private int timer;
        private int[] tin;
        private int[] fup;
        private bool[] used;
        private int[,] mx;
        private int nodes, edges;
        Node[] graph;

        void new_graph(int num_of_nodes, int num_of_edges)
        {
            this.nodes = num_of_nodes;
            this.edges = num_of_edges;
            this.graph = new Node[nodes];
            for (int i = 0; i < nodes; i++)
            {
                this.graph[i] = new Node(i);

            }

            int count = 0;
            for (int i = 0; i < nodes; i++)
            {
                bool flag = true;
                if (count < edges)
                {
                    while (flag)
                    {
                        Console.WriteLine("Add neighbor for node " + (i + 1));
                        int node = int.Parse(Console.ReadLine());
                        if (node <= nodes && count <= edges)
                        {
                            graph[i].edges.Add(graph[node - 1]);
                            count++;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect num of node or num of edge");
                            break;
                        }

                        Console.WriteLine("Add one more neighbor for node " + (i + 1) + "?(y/n)");
                        char anwser = char.Parse(Console.ReadLine());
                        if (anwser == 'y' || anwser == 'Y')
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }
            }
        }

        void test_graph()
        {
            this.nodes = 7;
            this.edges = 16;
            this.graph = new Node[nodes];
            for (int i = 0; i < nodes; i++)
            {
                this.graph[i] = new Node(i);
            }

            int[,] ed = {{0, 1},
                         {0, 6},
                         {1, 0},
                         {1, 6},
                         {2, 6},
                         {3, 4},
                         {3, 6}, 
                         {4, 3},
                         {4, 6},
                         {5, 6},
                         {6, 0},
                         {6, 1},
                         {6, 2},
                         {6, 3},
                         {6, 4},
                         {6, 5}};
            for (int i = 0; i < this.edges; i++)
            {
                graph[ed[i, 0]].edges.Add(graph[ed[i, 1]]);
            }
        }

        private void build_matrix()
        {
            this.mx = new int [this.nodes, this.nodes];
            for (int i = 0; i < this.nodes; i++)
            {
                for (int j = 0; j < this.nodes; j++)
                {
                    this.mx[i, j] = 0;
                }

                foreach (Node node in this.graph[i].edges)
                {
                    this.mx[i, node.index] = 1;
                }
            }
        }

        private void dfs(int v, int p = -1) 
        {
            this.used[v] = true;
            this.tin[v] = this.fup[v] = this.timer++;
            for (int i = 0; i < this.nodes; ++i)
            {
                if (this.mx[v, i] == 1)
                {
                    int to = i;
                    if (to == p)
                    {
                        continue;
                    }

                    if (used[to])
                    {
                        fup[v] = Math.Min(fup[v], tin[to]);
                    }
                    else
                    {
                        dfs(to, v);
                        fup[v] = Math.Min(fup[v], fup[to]);
                        if (fup[to] > tin[v])
                        {
                            Console.WriteLine("bridge: (" + (v + 1) + " , " + (to + 1) + ")");
                            Node[] bridge = {graph[v], graph[to]};
                            briges.Add(bridge);
                        }
                    }
                }
            }
        }

        private void find_bridges() 
        {
            this.timer = 0;
            this.used = new bool[nodes];
            this.fup = new int[nodes];
            this.tin = new int[nodes];

            for (int i = 0; i < this.nodes; ++i)
            {
                this.used[i] = false;
            }

            for (int i = 0; i < this.nodes; ++i)
            {
                if (!this.used[i])
                {
                    dfs(i);
                }
            }
        }

        void print()
        {
            for (int i = 0; i < nodes; i++)
            {
                Console.WriteLine("Node " + (i + 1) + ": " + graph[i].edges.ToString());
            }
        }

        int[] num_of_nodes_and_edges()
        {

            Console.WriteLine("Input num of nodes: ");
            int num_of_nodes = int.Parse(Console.ReadLine());
            Console.ReadKey();
            Console.WriteLine("Input num of edges: ");
            int num_of_edges = int.Parse(Console.ReadLine());
            Console.ReadKey();
            Console.Clear();
            int[] result = {num_of_nodes, num_of_edges};
            Console.WriteLine("Num of nodes: " + num_of_nodes + "\nNum of edges: " + num_of_edges);
            return result;
        }

        void remove_nodes()
        {
            for (int i = 0; i < nodes; i++)
            {
                graph[i].pow = graph[i].edges.Count;
            }
            ArrayList reference_point = new ArrayList();
            foreach (Node[] br in this.briges)
            {
                reference_point.Add(br[0].index);
                reference_point.Add(br[1].index);
            }

            int index_to_remove = -1;
            for (int i = 0; i < nodes; i++)
            {
                bool to_delete = false;
                if (graph[i].exist)
                {
                    foreach (int o in reference_point)
                    {
                        if (graph[i].index == o)
                        {
                            to_delete = false;
                            break;
                        }
                        else
                        {
                            to_delete = true;
                        }
                    }

                    if (to_delete)
                    {
                        index_to_remove = i;
                        break;
                    }
                }
            }

            if (index_to_remove >= 0 && index_to_remove < nodes)
            {
                for (int i = 0; i < this.nodes; i++)
                {
                    graph[i].edges.Remove(graph[index_to_remove]);
                }

                graph[index_to_remove].exist = false;
                graph[index_to_remove].edges.Clear();
                edges--;
                deleted_nodes.Push(graph[index_to_remove]);
            }
            else
            {
                index_to_remove = nodes + 1;
                for (int i = 0; i < nodes; i++)
                {
                    if (graph[i].exist)
                    {
                        index_to_remove = Math.Min(index_to_remove, graph[i].index);
                    }
                }
                if (index_to_remove >= 0 && index_to_remove < nodes)
                {
                    for (int i = 0; i < this.nodes; i++)
                    {
                        graph[i].edges.Remove(graph[index_to_remove]);
                    }

                    graph[index_to_remove].exist = false;
                    graph[index_to_remove].edges.Clear();
                    edges--;
                    deleted_nodes.Push(graph[index_to_remove]);
                }
                else
                {
                    Console.WriteLine("IS NOTHING TO REMOVE!");
                }
            }
        }

        public void stack_output()
        {
            Console.WriteLine("Stack of deleted nodes: ");
            foreach (Node node in this.deleted_nodes)
            {
                Console.Write((node.index + 1) + " ");
            }
        }

        public void matrix_output()
        {
            Console.WriteLine("\nMatrix: ");
            for (int i = 0; i < nodes; i++)
            {
                for (int j = 0; j < nodes; j++)
                {
                    Console.Write(mx[i, j] + " ");
                }

                Console.Write("\n");
            }
        }

        public static void Main(string[] args)
        {
            Program lab1 = new Program();
            Console.WriteLine("Choose tipe of graph input: \n\t1)User graph\n\t2)Default graph G = (7,16)");
            int mark = int.Parse(Console.ReadLine());
            if (mark == 1)
            {
                int[] count = lab1.num_of_nodes_and_edges();
                lab1.new_graph(count[0],count[1]);
                for (int i = 0; i < count[0]; i++)
                {
                    lab1.build_matrix();
                    lab1.matrix_output();
                    lab1.find_bridges();
                    lab1.remove_nodes();
                    lab1.stack_output();
                }
            }else{
                if (mark == 2)
                {
                    lab1.test_graph();
                    for (int i = 0; i < 7; i++)
                    {
                        lab1.build_matrix();
                        lab1.matrix_output();
                        lab1.find_bridges();
                        lab1.remove_nodes();
                        lab1.stack_output();
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect choose. Restart programm and try again!");
                }
            }

        }
    }
}