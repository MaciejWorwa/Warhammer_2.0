using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float minZoomSize = 4f;
    public float maxZoomSize = 15f;
    public Vector2 maxXRange = new Vector2(-40f, 40f);
    public Vector2 maxYRange = new Vector2(-20f, 20f);

    private Vector3 dragOrigin;

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Oblicza nowy rozmiar pola widzenia kamery
        float newZoomSize = Camera.main.orthographicSize - scrollInput * moveSpeed;

        // Ogranicza rozmiar pola widzenia kamery do ustalonych granic
        newZoomSize = Mathf.Clamp(newZoomSize, minZoomSize, maxZoomSize);

        // Aktualizuje rozmiar pola widzenia kamery
        Camera.main.orthographicSize = newZoomSize;

        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Sprawdza, czy klawisze W, A, S lub D s¹ naciœniête
        if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            // Pobiera wejœcie z klawiszy strza³ek
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        // Oblicza wektor ruchu dla przesuniêcia kamery
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f) * moveSpeed * Time.deltaTime;

        // Sprawdza, czy ruch przekracza zakresy i ogranicza go do zakresów
        float newX = Mathf.Clamp(transform.position.x + moveDirection.x, maxXRange.x, maxXRange.y);
        float newY = Mathf.Clamp(transform.position.y + moveDirection.y, maxYRange.x, maxYRange.y);
        moveDirection = new Vector3(newX, newY, -10f) - transform.position;

        // Przesuwa kamerê
        transform.Translate(moveDirection);
    }
    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(2)) // Sprawdza, czy œrodkowy przycisk myszy zosta³ wciœniêty
        {
            dragOrigin = Input.mousePosition; // Pobiera pozycjê myszy w momencie wciœniêcia œrodkowego przycisku
            return;
        }

        if (Input.GetMouseButton(2)) // Sprawdza, czy œrodkowy przycisk myszy jest przytrzymany
        {
            Vector3 currentPosition = Input.mousePosition; // Aktualna pozycja myszy
            Vector3 dragDirection = (currentPosition - dragOrigin) * (600f * moveSpeed / Screen.width) * Time.deltaTime; // Kierunek przesuniêcia

            transform.Translate(-dragDirection); // Przesuwa kamerê przeciwnie do kierunku przeci¹gniêcia

            dragOrigin = currentPosition; // Aktualizuje pozycjê pocz¹tkow¹ dla kolejnego kroku
        }

        // Sprawdza, czy klawisze W, A, S lub D zosta³y zwolnione
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            // Resetuje wejœcie
            Input.ResetInputAxes();
        }
    }
}
