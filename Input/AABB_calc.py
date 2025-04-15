import argparse

def parse_ply(file_path):
    """
    Parses an ASCII PLY file and returns a list of vertices.
    Each vertex is a tuple (x, y, z). It assumes that the first three
    numbers on a vertex line are the position.
    """
    vertices = []
    vertex_count = None
    header_end_index = None

    with open(file_path, 'r') as f:
        lines = f.readlines()

    # Process the header
    for i, line in enumerate(lines):
        line = line.strip()
        if line.startswith("element vertex"):
            parts = line.split()
            vertex_count = int(parts[-1])
        elif line.startswith("end_header"):
            header_end_index = i
            break

    if vertex_count is None or header_end_index is None:
        raise Exception("Invalid PLY file format: Could not determine vertex count or header end.")

    # Process vertex data (skip other properties beyond x, y, z)
    for line in lines[header_end_index + 1: header_end_index + 1 + vertex_count]:
        parts = line.split()
        # Parse only the first three values as floats for x, y, and z.
        x, y, z = map(float, parts[:3])
        vertices.append((x, y, z))

    return vertices

def calculate_aabb(vertices):
    """
    Computes the Axis-Aligned Bounding Box (AABB) for a list of vertices.
    
    Returns:
        min_point: (min_x, min_y, min_z)
        max_point: (max_x, max_y, max_z)
        center: (center_x, center_y, center_z)
    """
    xs = [v[0] for v in vertices]
    ys = [v[1] for v in vertices]
    zs = [v[2] for v in vertices]

    min_point = (min(xs), min(ys), min(zs))
    max_point = (max(xs), max(ys), max(zs))
    center = ((min_point[0] + max_point[0]) / 2,
              (min_point[1] + max_point[1]) / 2,
              (min_point[2] + max_point[2]) / 2)
    return min_point, max_point, center

def main():
    parser = argparse.ArgumentParser(description="Calculate the AABB for a given ASCII PLY file.")
    parser.add_argument("ply_file", help="Path to the PLY file")
    args = parser.parse_args()

    vertices = parse_ply(args.ply_file)
    if not vertices:
        print("No vertices found in the file.")
        return

    min_point, max_point, center = calculate_aabb(vertices)
    print("AABB Minimum:", min_point)
    print("AABB Maximum:", max_point)
    print("AABB Center: ", center)

if __name__ == "__main__":
    main()
